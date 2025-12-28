using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class CageLockInteractable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CageDoorController cageDoor;
    [SerializeField] private Renderer lockRenderer;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material highlightMaterial;

    private XRSimpleInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        interactable.selectEntered.AddListener(OnSelect);
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        lockRenderer.material = highlightMaterial;
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        lockRenderer.material = defaultMaterial;
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        cageDoor.ToggleDoor();
    }

    void OnDestroy()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
        interactable.selectEntered.RemoveListener(OnSelect);
    }
}
