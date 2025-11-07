using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class VisceralDescription : MonoBehaviour
{
    [Header("Visceral Info")]
    public string visceralTitle;           // e.g. "Femur"
    [TextArea] public string visceralInfo; // detailed description

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
        if (titleText != null) titleText.text = visceralTitle;
        if (descriptionText != null) descriptionText.text = visceralInfo;
    }

    private void ClearText()
    {
        if (titleText != null) titleText.text = "Visceral System";
        if (descriptionText != null)   descriptionText.text = "Welcome to PAWSIBLE! This is the visceral system of the dog. These internal organs keep the body alive by helping with breathing, digestion, blood circulation, and waste removal. Explore each organ to learn its name, location, and function.";
    }
}
