using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class Socket_Validator : MonoBehaviour
{
    [Header("Allowed Bone Tag")]
    public string allowedTag; // e.g., "Tibia"

    private XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDestroy()
    {
        socket.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Only allow correct bone to snap
        if (!args.interactableObject.transform.CompareTag(allowedTag))
        {
            // Cancel selection
            socket.interactionManager?.CancelInteractableSelection(args.interactableObject);
        }
    }
}
