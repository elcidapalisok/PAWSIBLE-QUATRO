using UnityEngine;

public class WoundStainReceiver : MonoBehaviour
{
    [Header("Mask Target")]
    public RenderTexture stainMaskRT;

    [Header("Brush")]
    public Material brushMaterial;
    [Range(0.001f, 0.2f)] public float brushSizeUV = 0.03f;
    [Range(0f, 2f)] public float brushStrength = 0.25f;
    [Range(0f, 1f)] public float hardness = 0.5f;

    RenderTexture _temp;

    void Awake()
    {
        ClearMask();
    }

    public void ClearMask()
    {
        if (stainMaskRT == null) return;
        var active = RenderTexture.active;
        RenderTexture.active = stainMaskRT;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = active;
    }

    public void PaintAtUV(Vector2 uv)
    {
        if (stainMaskRT == null || brushMaterial == null) return;

        if (_temp == null || _temp.width != stainMaskRT.width || _temp.height != stainMaskRT.height)
        {
            if (_temp != null) _temp.Release();
            _temp = new RenderTexture(stainMaskRT.width, stainMaskRT.height, 0, stainMaskRT.format);
            _temp.Create();
        }

        brushMaterial.SetVector("_BrushPos", new Vector4(uv.x, uv.y, 0, 0));
        brushMaterial.SetFloat("_BrushSize", brushSizeUV);
        brushMaterial.SetFloat("_BrushStrength", brushStrength);
        brushMaterial.SetFloat("_Hardness", hardness);

        Graphics.Blit(stainMaskRT, _temp);
        Graphics.Blit(_temp, stainMaskRT, brushMaterial);
    }

    void OnDestroy()
    {
        if (_temp != null) _temp.Release();
    }
}
