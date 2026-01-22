using UnityEngine;


public class CapAttachmentLink : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor Socket { get; private set; }
    public UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable Interactable { get; private set; }

    public void Set(UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket, UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
    {
        Socket = socket;
        Interactable = interactable;
    }

    public void Clear()
    {
        Socket = null;
        Interactable = null;
    }

    public bool IsAttached => Socket != null && Interactable != null;
}
