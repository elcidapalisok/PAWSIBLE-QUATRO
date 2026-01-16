// SceneObjectHighlighter.cs
using UnityEngine;

public class SceneObjectHighlighter : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 2f;
    
    private Renderer objectRenderer;
    private Material originalMaterial;
    private bool isHighlighted = false;
    
    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            objectRenderer = GetComponentInChildren<Renderer>();
        }
    }
    
    public void Highlight()
    {
        if (objectRenderer == null || isHighlighted) return;
        
        originalMaterial = objectRenderer.material;
        
        // Create highlight material
        Material highlightMat = new Material(originalMaterial);
        
        if (highlightMat.HasProperty("_EmissionColor"))
        {
            highlightMat.EnableKeyword("_EMISSION");
            highlightMat.SetColor("_EmissionColor", highlightColor * highlightIntensity);
        }
        else if (highlightMat.HasProperty("_Color"))
        {
            // Alternative: Tint the color
            highlightMat.color = Color.Lerp(highlightMat.color, highlightColor, 0.5f);
        }
        
        objectRenderer.material = highlightMat;
        isHighlighted = true;
        Debug.Log($"Highlighted: {gameObject.name}");
    }
    
    public void RemoveHighlight()
    {
        if (objectRenderer == null || !isHighlighted || originalMaterial == null) return;
        
        objectRenderer.material = originalMaterial;
        isHighlighted = false;
        Debug.Log($"Removed highlight from: {gameObject.name}");
    }
}