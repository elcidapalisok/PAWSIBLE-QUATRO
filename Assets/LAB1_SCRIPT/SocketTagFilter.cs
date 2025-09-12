using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class SocketBoneFilter : MonoBehaviour
{
    public string allowedTag; // e.g., "Tibia"
    private XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(ValidateBone);
    }

    void OnDestroy()
    {
        socket.selectEntered.RemoveListener(ValidateBone);
    }

    private void ValidateBone(SelectEnterEventArgs args)
    {
        if (!args.interactableObject.transform.CompareTag(allowedTag))
        {
            // Reject the wrong bone
            socket.interactionManager?.CancelInteractableSelection(args.interactableObject);
        }
    }
}
