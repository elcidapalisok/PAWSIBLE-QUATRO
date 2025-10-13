using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class XRAudioToggle : MonoBehaviour
{
    public AudioSource audioSource;

    private XRBaseInteractable interactable;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Ensure it doesn't play on start
        audioSource.playOnAwake = false;
        audioSource.loop = true;

        interactable = GetComponent<XRBaseInteractable>();

        // Subscribe to XR select event (click or press)
        interactable.selectEntered.AddListener(OnToggle);
    }

    private void OnToggle(SelectEnterEventArgs args)
    {
        if (audioSource.isPlaying)
            audioSource.Stop();   // ⛔ stop immediately
        else
            audioSource.Play();   // ▶️ start looping
    }

    void OnDestroy()
    {
        // Clean up listener
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnToggle);
    }
}
