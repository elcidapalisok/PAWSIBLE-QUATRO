using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance; // Singleton for easy access

    [Header("UI References")]
    public Canvas tooltipCanvas;
    public TextMeshProUGUI tooltipText;

    [Header("Settings")]
    public float fadeSpeed = 5f;

    private CanvasGroup canvasGroup;
    private bool isVisible = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        canvasGroup = tooltipCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = tooltipCanvas.gameObject.AddComponent<CanvasGroup>();

        HideTooltipInstant();
    }

    void Update()
    {
        // Optional smooth fade
        float targetAlpha = isVisible ? 1f : 0f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    }

    public void ShowTooltip(string message)
    {
        tooltipText.text = message;
        isVisible = true;
    }

    public void HideTooltip()
    {
        isVisible = false;
    }

    public void HideTooltipInstant()
    {
        isVisible = false;
        canvasGroup.alpha = 0;
    }
}
