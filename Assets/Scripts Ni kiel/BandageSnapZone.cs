using UnityEngine;


public class BandageSnapZone : MonoBehaviour
{
    public Transform snapPoint;
    public string bandageTag = "BandagePad";
    private bool placed;

    private void OnTriggerEnter(Collider other)
    {
        if (placed) return;

        if (!other.CompareTag(bandageTag))
            return;

        placed = true;

        Transform padRoot = other.transform.root;

        padRoot.position = snapPoint.position;
        padRoot.rotation = snapPoint.rotation;

        Rigidbody rb = padRoot.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = padRoot.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null) grab.enabled = false;

        Debug.Log("Bandage placed");
    }
}
