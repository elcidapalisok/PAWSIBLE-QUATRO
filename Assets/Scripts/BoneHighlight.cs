using UnityEngine;

public class BoneHighlight : MonoBehaviour
{
    public Renderer targetRenderer;
    public Color highlightColor = Color.cyan;
    public float emissionIntensity = 1.5f;

    MaterialPropertyBlock mpb;
    int emissionID;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        mpb = new MaterialPropertyBlock();
        emissionID = Shader.PropertyToID("_EmissionColor");

        if (targetRenderer != null && targetRenderer.sharedMaterial != null)
            targetRenderer.sharedMaterial.EnableKeyword("_EMISSION");
    }

    public void EnableHighlight()
    {
        SetEmission(highlightColor * emissionIntensity);
    }

    public void DisableHighlight()
    {
        SetEmission(Color.black); // no emission
    }

    void SetEmission(Color color)
    {
        if (targetRenderer == null) return;
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(emissionID, color);
        targetRenderer.SetPropertyBlock(mpb);
    }
}
