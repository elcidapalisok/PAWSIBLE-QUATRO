using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CapSnapCarry : MonoBehaviour
{
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;
    [SerializeField] private Transform parentWhenAttached; // ideally a scale=1 root
    [SerializeField] private bool disableCapGrabWhileAttached = true;

    private void Reset()
    {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        parentWhenAttached = transform;
    }

    private void Awake()
    {
        if (!socket) socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        if (!parentWhenAttached) parentWhenAttached = transform;

        socket.selectEntered.AddListener(OnAttached);
        socket.selectExited.AddListener(OnDetached);
    }

    private void OnDestroy()
    {
        socket.selectEntered.RemoveListener(OnAttached);
        socket.selectExited.RemoveListener(OnDetached);
    }

    private void OnAttached(SelectEnterEventArgs args)
    {
        var cap = args.interactableObject.transform;

        // Link so grabbing cap can detach
        var link = cap.GetComponent<CapAttachmentLink>();
        if (!link) link = cap.gameObject.AddComponent<CapAttachmentLink>();
        link.Set(socket, args.interactableObject);

        // Delay the parenting to avoid fighting with XR snap in the same frame
        StartCoroutine(ParentNextFrame(cap));
    }

    private IEnumerator ParentNextFrame(Transform cap)
    {
        yield return null; // wait 1 frame so XR socket finishes aligning

        if (!cap) yield break;

        // Freeze world pose, then parent without changing it
        Vector3 pos = cap.position;
        Quaternion rot = cap.rotation;

        cap.SetParent(parentWhenAttached, true);
        cap.position = pos;
        cap.rotation = rot;

        // Make physics stable while attached
        var rb = cap.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        // Prevent cap�s grab logic from trying to �own� parent/pose while attached
        if (disableCapGrabWhileAttached)
        {
            var capGrab = cap.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (capGrab) capGrab.trackPosition = false; // stops driving pose
            if (capGrab) capGrab.trackRotation = false;
        }
    }

    private void OnDetached(SelectExitEventArgs args)
    {
        var cap = args.interactableObject.transform;

        // Restore grab tracking
        var capGrab = cap.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (capGrab)
        {
            capGrab.trackPosition = true;
            capGrab.trackRotation = true;
        }

        // Unparent
        cap.SetParent(null, true);

        var rb = cap.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;

        var link = cap.GetComponent<CapAttachmentLink>();
        if (link) link.Clear();
    }
}
