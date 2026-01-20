using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WipeZoneAutoFit : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer towelRenderer;
    [Tooltip("Extra padding around the towel (local units).")]
    [Min(0f)] public float padding = 0.02f;

    private BoxCollider box;

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    private void Start()
    {
        FitOnce();
    }

    public void FitOnce()
    {
        if (towelRenderer == null)
        {
            Debug.LogError("[WipeZoneAutoFit] towelRenderer not assigned.");
            return;
        }

        // Renderer bounds are WORLD space AABB.
        Bounds wb = towelRenderer.bounds;

        // Convert world bounds center to local space of the wipe zone object
        Vector3 localCenter = transform.InverseTransformPoint(wb.center);

        // Approximate local size by inverse scaling the world extents
        // (works well if WipeZone is parented under the towel root with consistent scaling)
        Vector3 worldSize = wb.size;
        Vector3 lossy = transform.lossyScale;
        Vector3 localSize = new Vector3(
            lossy.x != 0f ? worldSize.x / lossy.x : worldSize.x,
            lossy.y != 0f ? worldSize.y / lossy.y : worldSize.y,
            lossy.z != 0f ? worldSize.z / lossy.z : worldSize.z
        );

        localSize += Vector3.one * padding;

        box.center = localCenter;
        box.size = localSize;
    }
}
