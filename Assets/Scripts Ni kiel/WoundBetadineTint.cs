using UnityEngine;

public class WoundBetadineTint : MonoBehaviour
{
    public Color betadineColor = new Color(0.45f, 0.25f, 0.08f, 1f);
    [Range(0f, 1f)] public float targetAmount = 1f;

    [SerializeField, Range(0f, 1f)] private float amount;

    public Renderer targetRenderer;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private MaterialPropertyBlock mpb;
    private Color startColor;
    private bool hasBaseColor;
    private bool hasColor;

    private void Awake()
    {
        if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();

        var mat = targetRenderer.sharedMaterial;
        hasBaseColor = mat != null && mat.HasProperty(BaseColorId);
        hasColor = mat != null && mat.HasProperty(ColorId);

        if (hasBaseColor) startColor = mat.GetColor(BaseColorId);
        else if (hasColor) startColor = mat.GetColor(ColorId);
        else startColor = Color.white;
    }

    public void ApplyBetadine(float add)
    {
        amount = Mathf.Clamp01(amount + add);
        float t = Mathf.Clamp01(amount / Mathf.Max(0.0001f, targetAmount));
        SetTint(t);
    }

    private void SetTint(float t)
    {
        Color blended = Color.Lerp(startColor, betadineColor, t);

        targetRenderer.GetPropertyBlock(mpb);
        if (hasBaseColor) mpb.SetColor(BaseColorId, blended);
        if (hasColor) mpb.SetColor(ColorId, blended);
        targetRenderer.SetPropertyBlock(mpb);
    }
}
