using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketParentOnSelect : MonoBehaviour
{
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;
    [SerializeField] private Transform parentWhenSelected; // set to needle root (usually this.transform)

    private void Reset()
    {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        parentWhenSelected = transform;
    }

    private void Awake()
    {
        if (!socket) socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        if (!parentWhenSelected) parentWhenSelected = transform;

        socket.selectEntered.AddListener(OnSelectEntered);
        socket.selectExited.AddListener(OnSelectExited);
    }

    private void OnDestroy()
    {
        socket.selectEntered.RemoveListener(OnSelectEntered);
        socket.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        var cap = args.interactableObject.transform;

        // Parent cap to needle so it moves with the needle
        cap.SetParent(parentWhenSelected, true);

        // Optional: make cap rigidbody kinematic while attached
        var rb = cap.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        var cap = args.interactableObject.transform;

        // Unparent cap
        cap.SetParent(null, true);

        // Optional: restore rigidbody
        var rb = cap.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;
    }
}
