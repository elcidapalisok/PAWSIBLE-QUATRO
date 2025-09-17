using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CanvasInteractable : MonoBehaviour
{
    public static bool isCanvasActivated = false;
    private XRSimpleInteractable simpleInteractable;

    void Awake()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        simpleInteractable.selectEntered.AddListener(OnCanvasTap);
    }

    private void OnCanvasTap(SelectEnterEventArgs args)
    {
        if (isCanvasActivated) return; // already activated once

        isCanvasActivated = true;
        Debug.Log("Canvas activated! Bones can now be grabbed/hovered.");

        // Disable further interaction
        simpleInteractable.enabled = false;
    }
}
