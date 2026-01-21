using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class DetachCapOnlyWhenGrabbed : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    private void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.selectEntered.AddListener(OnCapGrabbed);
    }

    private void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnCapGrabbed);
    }

    private void OnCapGrabbed(SelectEnterEventArgs args)
    {
        var link = GetComponent<CapAttachmentLink>();
        if (link == null || !link.IsAttached)
            return;

        // Force the socket to release this cap
        var mgr = link.Socket.interactionManager;
        if (mgr != null)
        {
            mgr.SelectExit(link.Socket, link.Interactable);
        }

        // Unparent so it becomes independent
        transform.SetParent(null, true);

        // Restore physics
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;

        link.Clear();
    }
}
