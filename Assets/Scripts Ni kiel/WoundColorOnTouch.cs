using UnityEngine;

public class WoundColorOnTouch : MonoBehaviour
{
    public Renderer woundRenderer;
    public Material betadineMaterial;
    public string iodineTipTag = "IodineTip";

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[WoundZone] OnTriggerEnter with: {other.name} | tag={other.tag}");
        TryApply(other);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"[WoundZone] OnTriggerStay with: {other.name} | tag={other.tag}");
        TryApply(other);
    }

    void TryApply(Collider other)
    {
        if (!other.CompareTag(iodineTipTag)) return;

        Debug.Log("[WoundZone] Tag matched. Applying Betadine material.");

        if (woundRenderer == null) Debug.LogError("[WoundZone] woundRenderer is NULL");
        if (betadineMaterial == null) Debug.LogError("[WoundZone] betadineMaterial is NULL");

        if (woundRenderer != null && betadineMaterial != null)
            woundRenderer.material = betadineMaterial;
    }
}
