using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
public class AttachablePart : MonoBehaviour
{
    [Header("Attachment Settings")]
    public string attachTag = "Socket";  // The tag of the socket it belongs to
    public bool isAttached = true;
    public Transform defaultParent; // Original parent (like Syringe Body)
    public float snapRange = 0.05f; // how close before snapping in

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private Transform currentSocket;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Initially attached parts don't have their own physics
        if (isAttached)
        {
            if (rb != null) rb.isKinematic = true;
        }
    }

    void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (isAttached)
        {
            Detach();
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        TryAttachToNearbySocket();
    }

    void Detach()
    {
        isAttached = false;
        transform.parent = null;

        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        Debug.Log($"{gameObject.name} detached from {defaultParent?.name}");
    }

    void TryAttachToNearbySocket()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, snapRange);
        foreach (var col in nearby)
        {
            if (col.CompareTag(attachTag))
            {
                currentSocket = col.transform;
                Attach(currentSocket);
                return;
            }
        }

        // No valid socket found → remain loose
        Debug.Log($"{gameObject.name} released, no socket nearby.");
    }

    void Attach(Transform socket)
    {
        transform.SetParent(socket);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (rb != null)
        {
            Destroy(rb);
        }

        isAttached = true;
        Debug.Log($"{gameObject.name} attached to {socket.name}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, snapRange);
    }
}
