using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class BoneDescription : MonoBehaviour
{
    [Header("Bone Info")]
    public string boneTitle;           // e.g. "Femur"
    [TextArea] public string boneInfo; // detailed description

    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    private XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;
    private bool ready = false; // prevents early triggering

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.hoverEntered.AddListener(OnHoverEnter);
        grabInteractable.hoverExited.AddListener(OnHoverExit);
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void Start()
    {
        // Delay one frame to prevent auto-trigger on scene start
        StartCoroutine(EnableAfterDelay());
    }

    IEnumerator EnableAfterDelay()
    {
        yield return null; // wait 1 frame
        ready = true;
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (!ready || isGrabbed) return;
        ShowText();
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (!ready || isGrabbed) return;
        ClearText();
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!ready) return;
        isGrabbed = true;
        ShowText();
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (!ready) return;
        isGrabbed = false;
        ClearText();
    }

    private void ShowText()
    {
        if (titleText != null) titleText.text = boneTitle;
        if (descriptionText != null) descriptionText.text = boneInfo;
    }

    private void ClearText()
    {
        if (titleText != null) titleText.text = "Skeletal structure ";
        if (descriptionText != null) descriptionText.text = "Welcome to PAWSIBLE! This is the skeletal system of the dog. The skeleton provides structure, protects vital organs, and supports movement. Explore each bone to learn its name, location, and function";
    }
}
