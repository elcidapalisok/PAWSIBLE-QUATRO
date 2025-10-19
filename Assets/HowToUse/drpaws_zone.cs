using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HologramEdgeEffect : MonoBehaviour
{
    public Material hologramMaterial;
    public float borderStrength = 3.0f;    // intensity of edge glow
    public float scrollSpeed = 1.0f;       // scanline speed
    public float flickerSpeed = 3.0f;      // how fast it flickers
    public float emissionIntensity = 2.5f; // glow power
    public Color borderColor = Color.cyan; // glow color

    private Renderer rend;
    private Camera cam;
    private Color baseColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (hologramMaterial == null)
            hologramMaterial = rend.material;

        baseColor = hologramMaterial.GetColor("_EmissionColor");
        cam = Camera.main;
    }

    void Update()
    {
        // --- Scanline Animation ---
        Vector2 offset = hologramMaterial.mainTextureOffset;
        offset.y += Time.deltaTime * scrollSpeed;
        hologramMaterial.mainTextureOffset = offset;

        // --- Flicker Effect ---
        float flicker = 0.7f + Mathf.Abs(Mathf.Sin(Time.time * flickerSpeed)) * 0.3f;

        // --- View-Dependent Border Glow (Fresnel Approximation) ---
        Vector3 viewDir = (cam.transform.position - transform.position).normalized;
        Vector3 normal = transform.forward;
        float fresnel = Mathf.Pow(1 - Vector3.Dot(viewDir, normal), borderStrength);

        // --- Final Emission ---
        Color emission = borderColor * fresnel * (emissionIntensity * flicker);
        hologramMaterial.SetColor("_EmissionColor", emission);

        // --- Transparency Pulse ---
        Color col = hologramMaterial.color;
        col.a = 0.4f + Mathf.Sin(Time.time * flickerSpeed) * 0.2f;
        hologramMaterial.color = col;
    }
}
