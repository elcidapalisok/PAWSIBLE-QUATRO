using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string objectName = "Interactable";
    public bool advanceDialogueOnUse = true;
    public Color highlightColor = Color.yellow;

    [Header("Dialogue Trigger Target")]
    public string targetSegmentName;
    public int targetLineIndex;

    [Header("References (Optional)")]
    [SerializeField] private DialogueManager dialogueManager;

    private XRBaseInteractable interactable;

    private Renderer objectRenderer;
    private MaterialPropertyBlock mpb;
    private static readonly int ColorProp = Shader.PropertyToID("_Color");

    private Color originalColor = Color.white;
    private bool hasColorProp = false;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
            objectRenderer = GetComponentInChildren<Renderer>();

        if (objectRenderer != null)
        {
            mpb = new MaterialPropertyBlock();
            objectRenderer.GetPropertyBlock(mpb);

            if (objectRenderer.sharedMaterial != null && objectRenderer.sharedMaterial.HasProperty(ColorProp))
            {
                hasColorProp = true;
                originalColor = objectRenderer.sharedMaterial.GetColor(ColorProp);
            }
        }

        if (string.IsNullOrEmpty(objectName))
            objectName = gameObject.name;

        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;

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
        SetHighlight(true);
        TooltipManager.Instance?.ShowTooltip(objectName);
    }

    public void OnHoverExit(HoverExitEventArgs args)
    {
        SetHighlight(false);
        TooltipManager.Instance?.HideTooltip();
    }

    private void SetHighlight(bool on)
    {
        if (objectRenderer == null || mpb == null || !hasColorProp) return;

        objectRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(ColorProp, on ? highlightColor : originalColor);
        objectRenderer.SetPropertyBlock(mpb);
    }

    public void OnSelect(SelectEnterEventArgs args)
    {
        if (dialogueManager == null) return;

        if (dialogueManager.IsAtStage(targetSegmentName, targetLineIndex))
        {
            if (advanceDialogueOnUse)
            {
                dialogueManager.AdvanceDialogue();
                Debug.Log($"{objectName} triggered dialogue ({DialogueManager.NormalizeSegmentKey(targetSegmentName)}:{targetLineIndex})");
            }
        }
        else
        {
            Debug.Log($"{objectName} interaction ignored (current {DialogueManager.NormalizeSegmentKey(dialogueManager.GetCurrentSegmentName())}:{dialogueManager.GetCurrentLineIndex()})");
        }
    }
}
