using UnityEngine;

public class BookPodiumZone : MonoBehaviour
{
    [SerializeField] private Transform snapPoint;

    // If snapping causes a brief trigger exit, this "sticks" the podium state.
    [SerializeField] private float exitGraceSeconds = 0.20f;

    private void OnTriggerEnter(Collider other)
    {
        TryPlace(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // IMPORTANT: if the book starts inside the zone OR briefly exits/re-enters while snapping,
        // this keeps podium state alive.
        TryPlace(other);
    }

    private void OnTriggerExit(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb == null) return;

        var book = rb.GetComponent<BookAnatomyController>();
        if (book != null) book.RemovedFromPodium(exitGraceSeconds);
    }

    private void TryPlace(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb == null) return;

        var book = rb.GetComponent<BookAnatomyController>();
        if (book != null) book.PlacedOnPodium(snapPoint);
    }
}
