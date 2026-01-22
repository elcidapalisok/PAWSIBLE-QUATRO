using UnityEngine;


[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class DisableSocketWhileHeld : MonoBehaviour
{
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    private void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (!socket) socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();

        grab.selectEntered.AddListener(_ => OnNeedleGrabbed());
        grab.selectExited.AddListener(_ => OnNeedleReleased());
    }

    private void OnEnable()
    {
        // Enable by default so snapping works when not held.
        SetSocket(true);
    }

    private void OnNeedleGrabbed()
    {
        if (!socket) return;

        // IMPORTANT: If the socket is currently holding the cap, DO NOT disable it.
        if (socket.hasSelection)
            return;

        SetSocket(false);
    }

    private void OnNeedleReleased()
    {
        SetSocket(true);
    }

    private void SetSocket(bool enabled)
    {
        if (!socket) return;
        socket.socketActive = enabled;
        socket.enabled = enabled;
    }
}
