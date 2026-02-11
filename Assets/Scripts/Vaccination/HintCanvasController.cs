using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HintCanvasController : MonoBehaviour
{
    [Header("UI References (HintCanvas)")]
    [SerializeField] private GameObject panelRoot;          // HintCanvas/Panel
    [SerializeField] private Image stepImage;               // HintCanvas/Panel/StepImage
    [SerializeField] private TextMeshProUGUI stepNameText;  // HintCanvas/Panel/StepName
    [SerializeField] private TextMeshProUGUI stepDescText;  // HintCanvas/Panel/StepDescription

    [Header("Hint Entries (configure in Inspector)")]
    [SerializeField] private List<HintEntry> hints = new List<HintEntry>();

    [Header("Behavior")]
    [Tooltip("If true, hides the panel when the current line does not require a trigger.")]
    [SerializeField] private bool hideWhenNotRequired = true;

    private Dictionary<(string segKey, int line), HintEntry> hintLookup;

    private string lastSegKey = null;
    private int lastLine = int.MinValue;

    [Serializable]
    public class HintEntry
    {
        [Tooltip("Must match DialogueSegment.segmentName. Spaces and case are ignored.")]
        public string segmentName;

        [Tooltip("Dialogue line index (0-based) that requires a trigger.")]
        public int lineIndex;

        [Header("Displayed Content")]
        public Sprite stepImage;
        public string stepName;

        [TextArea(2, 6)]
        public string stepDescription;
    }

    private void Awake()
    {
        BuildLookup();
    }

    private void OnValidate()
    {
        BuildLookup();
    }

    private void Start()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        ForceRefresh();
    }

    private void Update()
    {
        if (DialogueManager.Instance == null)
            return;

        string segKey = DialogueManager.NormalizeSegmentKey(
            DialogueManager.Instance.GetCurrentSegmentName()
        );
        int line = DialogueManager.Instance.GetCurrentLineIndex();

        if (segKey == lastSegKey && line == lastLine)
            return;

        lastSegKey = segKey;
        lastLine = line;

        RefreshUI(segKey, line);
    }

    private void ForceRefresh()
    {
        if (DialogueManager.Instance == null)
            return;

        string segKey = DialogueManager.NormalizeSegmentKey(
            DialogueManager.Instance.GetCurrentSegmentName()
        );
        int line = DialogueManager.Instance.GetCurrentLineIndex();

        lastSegKey = segKey;
        lastLine = line;

        RefreshUI(segKey, line);
    }

    private void BuildLookup()
    {
        hintLookup = new Dictionary<(string segKey, int line), HintEntry>();

        if (hints == null)
            return;

        foreach (HintEntry h in hints)
        {
            if (h == null)
                continue;

            string segKey = DialogueManager.NormalizeSegmentKey(h.segmentName);
            var key = (segKey, h.lineIndex);

            hintLookup[key] = h;
        }
    }

    private void RefreshUI(string segKey, int line)
    {
        bool requiresTrigger = IsTriggerRequired(segKey, line);

        if (!requiresTrigger)
        {
            if (hideWhenNotRequired && panelRoot != null)
                panelRoot.SetActive(false);

            return;
        }

        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (hintLookup != null &&
            hintLookup.TryGetValue((segKey, line), out HintEntry hint))
        {
            if (stepImage != null)
            {
                stepImage.sprite = hint.stepImage;
                stepImage.enabled = (hint.stepImage != null);
            }

            if (stepNameText != null)
                stepNameText.text = string.IsNullOrEmpty(hint.stepName)
                    ? "Step"
                    : hint.stepName;

            if (stepDescText != null)
                stepDescText.text = hint.stepDescription ?? "";
        }
        else
        {
            if (stepImage != null)
            {
                stepImage.sprite = null;
                stepImage.enabled = false;
            }

            if (stepNameText != null)
                stepNameText.text = "Step";

            if (stepDescText != null)
                stepDescText.text =
                    "No hint configured for " + segKey + ":" + line +
                    ". Add an entry in HintCanvasController.";
        }
    }

    private bool IsTriggerRequired(string segKey, int line)
    {
        switch (segKey)
        {
            case "handwashing":
                return line == 2 || line == 3 || line == 4 ||
                       line == 5 || line == 6 || line == 7;

            case "glovescoat":
                return line == 0 || line == 1;

            case "vaccineprep":
                return line == 0 || line == 3 || line == 4 ||
                       line == 6 || line == 7 || line == 9;

            case "injection":
                return line == 3 || line == 5 ||
                       line == 7 || line == 9;

            default:
                return false;
        }
    }
}
