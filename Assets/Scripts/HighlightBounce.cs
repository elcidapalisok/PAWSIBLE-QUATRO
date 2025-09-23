using UnityEngine;

public class HighlightBounce : MonoBehaviour
{
    public Renderer targetRenderer;     // Assign your Lab Coat's MeshRenderer
    public Color glowColor = Color.yellow; 
    public float bounceSpeed = 2f;      // How fast it bounces
    public float bounceAmount = 0.1f;   // How big the bounce is
    public float glowSpeed = 2f;        // How fast the glow pulses
    public float glowIntensity = 2f;    // Brightness of glow

    private Vector3 originalScale;
    private Material mat;

    void Start()
    {
        // Save original size
        originalScale = transform.localScale;

        // Get material
        if (targetRenderer != null)
        {
            mat = targetRenderer.material;
            mat.EnableKeyword("_EMISSION"); // Enable emission property
        }
    }

    void Update()
    {
        // 🔹 Bounce effect (scaling)
        float scale = 1 + Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;
        transform.localScale = originalScale * scale;

        // 🔹 Glow effect (pulsing emission)
        if (mat != null)
        {
            float emission = (Mathf.Sin(Time.time * glowSpeed) + 1f) / 2f; 
            Color finalColor = glowColor * (emission * glowIntensity);
            mat.SetColor("_EmissionColor", finalColor);
        }
    }
}
