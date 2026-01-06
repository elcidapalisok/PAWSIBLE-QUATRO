using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class BreakableVial : MonoBehaviour
{
    [Header("Vial Objects")]
    public GameObject wholeVial;
    public GameObject brokenVial;

    [Header("Break Settings")]
    public float breakVelocityThreshold = 2.5f;
    public float destroyDelay = 10f;

    [Header("Audio")]
    [Tooltip("Resources path to break sound (no extension)")]
    public string breakSfxResourcePath = "Audio/Vaccination_StoryMode/vial_break";

    private bool isBroken = false;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private AudioClip breakClip;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (brokenVial != null)
            brokenVial.SetActive(false);

        breakClip = Resources.Load<AudioClip>(breakSfxResourcePath);

        if (breakClip == null)
        {
            Debug.LogWarning(
                "BreakableVial: Could not load break SFX at Resources/" +
                breakSfxResourcePath
            );
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isBroken)
            return;

        if (grabInteractable != null && grabInteractable.isSelected)
            return;

        float impactVelocity = collision.relativeVelocity.magnitude;
        Debug.Log("[" + gameObject.name + "] Impact velocity: " + impactVelocity);

        if (impactVelocity >= breakVelocityThreshold)
        {
            BreakVial();
        }
    }

    void BreakVial()
    {
        if (isBroken)
            return;

        isBroken = true;

        if (grabInteractable != null && grabInteractable.isSelected)
        {
            grabInteractable.interactionManager.SelectExit(
                grabInteractable.firstInteractorSelecting,
                grabInteractable
            );
        }

        if (grabInteractable != null)
            grabInteractable.enabled = false;

        if (wholeVial != null)
            wholeVial.SetActive(false);

        if (brokenVial != null)
        {
            brokenVial.SetActive(true);

            foreach (Rigidbody shardRb in brokenVial.GetComponentsInChildren<Rigidbody>())
            {
                shardRb.isKinematic = false;
                shardRb.AddForce(
                    Random.insideUnitSphere * 0.6f,
                    ForceMode.Impulse
                );
            }
        }

        PlayBreakSound();

        Destroy(gameObject, destroyDelay);
    }

    void PlayBreakSound()
    {
        if (breakClip == null)
            return;

        GameObject audioObj = new GameObject("VialBreakSFX");
        audioObj.transform.position = transform.position;

        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.clip = breakClip;
        source.spatialBlend = 1f;
        source.playOnAwake = false;

        source.Play();
        Destroy(audioObj, breakClip.length);
    }
}
