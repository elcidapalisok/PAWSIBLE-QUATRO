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
    [Tooltip("Relative velocity needed to break (m/s).")]
    public float breakVelocityThreshold = 3.5f;

    [Tooltip("Minimum collision impulse required (helps ignore tiny contacts).")]
    public float minImpulseToBreak = 2.0f;

    [Tooltip("Ignore collisions for this many seconds after scene start/spawn.")]
    public float spawnGraceSeconds = 0.35f;

    [Tooltip("Ignore collisions for this many seconds after releasing from grab.")]
    public float releaseGraceSeconds = 0.25f;

    [Tooltip("Optional: require at least this much downward speed to break. Set to 0 to disable.")]
    public float minDownwardSpeedToBreak = 1.0f;

    [Tooltip("How many contacts must exist to consider it a real hit (1 is fine, 2 is stricter).")]
    public int minContactCount = 1;

    public float destroyDelay = 10f;

    [Header("Audio")]
    [Tooltip("Resources path to break sound (no extension)")]
    public string breakSfxResourcePath = "Audio/Vaccination_StoryMode/vial_break";

    private bool isBroken = false;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private AudioClip breakClip;

    private float spawnTime;
    private float lastReleaseTime = -999f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (brokenVial != null)
            brokenVial.SetActive(false);

        breakClip = Resources.Load<AudioClip>(breakSfxResourcePath);

        if (breakClip == null)
        {
            Debug.LogWarning("BreakableVial: Could not load break SFX at Resources/" + breakSfxResourcePath);
        }

        spawnTime = Time.time;

        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        lastReleaseTime = Time.time;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isBroken)
            return;

        // Grace period after spawn to avoid instant break from initial overlaps/settling
        if (Time.time - spawnTime < spawnGraceSeconds)
            return;

        // Do not break while held
        if (grabInteractable != null && grabInteractable.isSelected)
            return;

        // Grace period right after release (XR release often causes contact spikes)
        if (Time.time - lastReleaseTime < releaseGraceSeconds)
            return;

        // Contact count filter
        if (collision.contactCount < minContactCount)
            return;

        float impactVelocity = collision.relativeVelocity.magnitude;
        float impulse = collision.impulse.magnitude;

        // Optional downward speed requirement
        if (minDownwardSpeedToBreak > 0f)
        {
            float downwardSpeed = -rb.linearVelocity.y; // positive when moving down
            if (downwardSpeed < minDownwardSpeedToBreak)
                return;
        }

        // Debug (optional)
        // Debug.Log("[" + gameObject.name + "] vel=" + impactVelocity + " impulse=" + impulse + " contacts=" + collision.contactCount);

        // Require BOTH velocity and impulse (reduces false breaks massively)
        if (impactVelocity >= breakVelocityThreshold && impulse >= minImpulseToBreak)
        {
            BreakVial();
        }
    }

    void BreakVial()
    {
        if (isBroken)
            return;

        isBroken = true;

        // Force-release if somehow selected (extra safety)
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            if (grabInteractable.interactionManager != null && grabInteractable.firstInteractorSelecting != null)
            {
                grabInteractable.interactionManager.SelectExit(
                    grabInteractable.firstInteractorSelecting,
                    grabInteractable
                );
            }
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
                shardRb.AddForce(Random.insideUnitSphere * 0.6f, ForceMode.Impulse);
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
