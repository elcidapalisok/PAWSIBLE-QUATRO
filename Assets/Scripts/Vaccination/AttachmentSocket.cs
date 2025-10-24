using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttachmentSocket : MonoBehaviour
{
    [Header("Socket Settings")]
    public string acceptedTag = "Attachable";
    public bool showGizmo = true;

    void Awake()
    {
        // Ensure it’s a trigger collider
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
        gameObject.tag = "Socket";
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.03f);
    }
}
