using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class WoundInteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string objectName = "Interactable";
    public bool advanceDialogueOnUse = true;
    public Color highlightColor = Color.yellow;

    [Header("Dialogue Trigger Target")]
    public string targetSegmentName;
    public int targetLineIndex;

    private Color originalColor;
    private Renderer objectRenderer;
    private WoundDialogueManager dialogueManager;
    private XRBaseInteractable interactable;

    void Awake()
    {
        // Find the renderer either on this object or its children
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
            objectRenderer = GetComponentInChildren<Renderer>();

        if (objectRenderer != null)
            originalColor = objectRenderer.material.color;

        if (string.IsNullOrEmpty(objectName))
            objectName = gameObject.name;

        dialogueManager = FindObjectOfType<WoundDialogueManager>();
        interactable = GetComponent<XRBaseInteractable>();
    }

    void OnEnable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
            interactable.selectEntered.AddListener(OnSelect);
        }
    }

    void OnDisable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
            interactable.selectEntered.RemoveListener(OnSelect);
        }
    }

    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (objectRenderer != null)
            objectRenderer.material.color = highlightColor;

        TooltipManager.Instance?.ShowTooltip(objectName);
    }

    public void OnHoverExit(HoverExitEventArgs args)
    {
        if (objectRenderer != null)
            objectRenderer.material.color = originalColor;

        TooltipManager.Instance?.HideTooltip();
    }

    public void OnSelect(SelectEnterEventArgs args)
    {
        if (dialogueManager == null) return;

        string currentSegment = dialogueManager.GetCurrentSegmentName().ToLower().Trim();
        int currentLine = dialogueManager.GetCurrentLineIndex();
        string targetSegment = targetSegmentName.ToLower().Trim();

        if (currentSegment == targetSegment && currentLine == targetLineIndex)
        {
            if (advanceDialogueOnUse)
            {
                dialogueManager.AdvanceDialogue();
                Debug.Log($"{objectName} triggered dialogue ({targetSegment}:{targetLineIndex})");
            }
        }
        else
        {
            Debug.Log($"{objectName} interaction ignored — not the correct stage ({currentSegment}:{currentLine})");
        }
    }
}
