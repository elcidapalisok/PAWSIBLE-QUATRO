using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CageDoor : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Animator controlling the cage door")]
    public Animator doorAnimator;

    [Tooltip("XR Interactable on the DoorKnob child")]
    public XRBaseInteractable doorKnobInteractable;

    [Header("Audio")]
    [Tooltip("Resources path to metallic lock sound")]
    public string lockSfxResourcePath = "Audio/Vaccination_StoryMode/metallic_lock";

    private AudioClip lockClip;
    private bool isOpen = false;

    void Awake()
    {
        if (doorAnimator == null)
            doorAnimator = GetComponent<Animator>();

        if (doorKnobInteractable == null)
        {
            Debug.LogError(
                "CageDoor: DoorKnob XR Interactable reference is missing."
            );
        }

        lockClip = Resources.Load<AudioClip>(lockSfxResourcePath);

        if (lockClip == null)
        {
            Debug.LogWarning(
                "CageDoor: Could not load lock SFX at Resources/" +
                lockSfxResourcePath
            );
        }
    }

    void OnEnable()
    {
        if (doorKnobInteractable != null)
            doorKnobInteractable.selectEntered.AddListener(OnKnobSelected);
    }

    void OnDisable()
    {
        if (doorKnobInteractable != null)
            doorKnobInteractable.selectEntered.RemoveListener(OnKnobSelected);
    }

    void OnKnobSelected(SelectEnterEventArgs args)
    {
        if (isOpen)
            return;

        OpenDoor();
    }

    void OpenDoor()
    {
        isOpen = true;

        if (doorAnimator != null)
            doorAnimator.SetBool("isOpen", true);

        PlayLockSound();
    }

    void PlayLockSound()
    {
        if (lockClip == null)
            return;

        GameObject audioObj = new GameObject("CageDoorLockSFX");
        audioObj.transform.position = transform.position;

        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.clip = lockClip;
        source.spatialBlend = 1f;
        source.playOnAwake = false;
//
        source.Play();
        Destroy(audioObj, lockClip.length);
    }
}
