using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class BoneCrackController : MonoBehaviour
{
    public AudioSource crackSound;
    private XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrabbed);
        StartCoroutine(DisableAudioNextFrame());
    }

    IEnumerator DisableAudioNextFrame()
    {
        yield return null; // wait one frame so Timeline loads first
        if (crackSound != null)
        {
            crackSound.Stop();
            crackSound.playOnAwake = false;
            crackSound.enabled = false;
            crackSound.gameObject.SetActive(false); // fully disables it
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (crackSound != null)
        {
            crackSound.gameObject.SetActive(true);
            crackSound.enabled = true;
            crackSound.Stop();
            crackSound.Play();
        }
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrabbed);
    }
}
