using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class HighlightObject : MonoBehaviour
{
    [Header("Highlight Settings")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material highlightMaterial;

    private XRBaseInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();

        // Auto-find renderer if not assigned
        if (!targetRenderer)
            targetRenderer = GetComponent<Renderer>() ?? GetComponentInChildren<Renderer>();

        // Apply default material on start
        if (targetRenderer && defaultMaterial)
            targetRenderer.sharedMaterial = defaultMaterial;
    }

    void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
    }

    void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (targetRenderer && highlightMaterial)
            targetRenderer.sharedMaterial = highlightMaterial;
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (targetRenderer && defaultMaterial)
            targetRenderer.sharedMaterial = defaultMaterial;
    }
}
