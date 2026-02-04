using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class WDialogueManager : MonoBehaviour
{
    public static WDialogueManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public Image npcIcon;
    public Button nextButton;
    public Button prevButton;
    public Button skipButton;

    [Header("Dialogue Segments")]
    public List<DialogueSegment> dialogueSegments = new List<DialogueSegment>();

    private int currentSegmentIndex = 0;
    private int currentDialogueIndex = 0;
    private Coroutine typingCoroutine;
    private bool canProceed = true;

    [Header("Audio Settings")]
    public AudioSource voiceSource;

    [Tooltip("Optional fallback: Relative path inside Resources folder (e.g. 'Audio/Wound_StoryMode')")]
    public string audioFolderPath = "Audio/Wound_StoryMode";

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.03f;

    // (segmentKey, lineIndex) -> requires trigger
    private HashSet<(string, int)> triggerRequiredLines = new HashSet<(string, int)>();

    // (segmentKey, lineIndex) -> label from spreadsheet ("Trigger to proceed")
    private Dictionary<(string, int), string> triggerLabels = new Dictionary<(string, int), string>();

    [System.Serializable]
    public class DialogueSegment
    {
        public string segmentName;

        [TextArea(3, 10)]
        public List<string> dialogueLines = new List<string>();

        [Tooltip("Optional. If assigned, voiceClips[i] will be used for dialogueLines[i].")]
        public List<AudioClip> voiceClips = new List<AudioClip>();
    }

    [Header("Cutscene Control")]
    public bool cutsceneMode = false;

    [Header("NPC Animation (Optional)")]
    public Animator npcAnimator;
    public string isTalkingParam = "IsTalking";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple WDialogueManager instances found. Keeping the first instance.");
            return;
        }
        Instance = this;
    }

    void SetTalking(bool talking)
    {
        if (npcAnimator == null) return;
        npcAnimator.SetBool(isTalkingParam, talking);
    }

    void Start()
    {
        if (nextButton != null) nextButton.onClick.AddListener(NextDialogue);
        if (prevButton != null) prevButton.onClick.AddListener(PrevDialogue);
        if (skipButton != null) skipButton.onClick.AddListener(SkipDialogue);

        RegisterTriggerRequiredLines_FromSpreadsheetMapping();
        EnsureVoiceClipListSizes();

        if (dialogueSegments.Count > 0)
            ShowCurrentDialogue();
    }

    void OnValidate()
    {
        EnsureVoiceClipListSizes();
    }

    public static string NormalizeSegmentKey(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        // Match spreadsheet style: lowercase + trim + remove spaces only (keep underscores)
        return input.Trim().ToLowerInvariant().Replace(" ", "");
    }

    public bool IsAtStage(string segmentName, int lineIndex)
    {
        string currentSeg = NormalizeSegmentKey(GetCurrentSegmentName());
        string targetSeg = NormalizeSegmentKey(segmentName);
        return currentSeg == targetSeg && currentDialogueIndex == lineIndex;
    }

    void EnsureVoiceClipListSizes()
    {
        if (dialogueSegments == null) return;

        for (int i = 0; i < dialogueSegments.Count; i++)
        {
            var seg = dialogueSegments[i];
            if (seg == null) continue;

            if (seg.dialogueLines == null) seg.dialogueLines = new List<string>();
            if (seg.voiceClips == null) seg.voiceClips = new List<AudioClip>();

            int targetCount = seg.dialogueLines.Count;

            if (seg.voiceClips.Count < targetCount)
            {
                while (seg.voiceClips.Count < targetCount)
                    seg.voiceClips.Add(null);
            }
            else if (seg.voiceClips.Count > targetCount)
            {
                seg.voiceClips.RemoveRange(targetCount, seg.voiceClips.Count - targetCount);
            }
        }
    }

    /// <summary>
    /// This list is generated from your spreadsheet file "Wound - Dialogue Scripts.xlsx" (Sheet1):
    /// Any row where "Trigger to proceed" != "Nothing" becomes a trigger-required line.
    /// </summary>
    void RegisterTriggerRequiredLines_FromSpreadsheetMapping()
    {
        triggerRequiredLines.Clear();
        triggerLabels.Clear();

        // HANDWASHING (Trigger to proceed != Nothing)
        // If you don't have labels for these yet, the overload below will mark them as gated without a label.
        AddTrigger("handwashing", 2);
        AddTrigger("handwashing", 3);
        AddTrigger("handwashing", 4);
        AddTrigger("handwashing", 5);
        AddTrigger("handwashing", 6);
        AddTrigger("handwashing", 7);

        // GLOVES + COAT
        AddTrigger("glovescoat", 0, "Select Gloves");
        AddTrigger("glovescoat", 1, "Select a Lab Coat");

        // WOUND STABILIZATION PREP
        AddTrigger("woundstabilizationprep", 0, "Navigate beside the patient");
        AddTrigger("woundstabilizationprep", 2, "Navigate the cabinet tool");
        AddTrigger("woundstabilizationprep", 3, "Place object on tray");

        // WOUND STABILIZATION - CLEANING
        AddTrigger("woundstabilization_cleaning", 0, "Select Cotton");
        AddTrigger("woundstabilization_cleaning", 1, "Use Disinfectant on Cotton");
        AddTrigger("woundstabilization_cleaning", 2, "Use Cotton on Wound");

        // WOUND STABILIZATION - PRIMARY DRESSING
        AddTrigger("woundstabilization_primarydressing", 0, "Select Bandage Pad");
        AddTrigger("woundstabilization_primarydressing", 1, "Place Bandage Pad on Wound");

        // WOUND STABILIZATION - TERTIARY LAYER
        AddTrigger("woundstabilization_tertiarylayer", 0, "Select Cohesive Bandage");
        AddTrigger("woundstabilization_tertiarylayer", 1, "Wrap Cohesive Bandage");
    }

    // Overload: allow trigger-gated lines without a label
    void AddTrigger(string segmentKey, int lineIndex)
    {
        AddTrigger(segmentKey, lineIndex, "");
    }

    void AddTrigger(string segmentKey, int lineIndex, string label)
    {
        segmentKey = NormalizeSegmentKey(segmentKey);
        triggerRequiredLines.Add((segmentKey, lineIndex));

        // Store label only if you want; empty label is fine.
        triggerLabels[(segmentKey, lineIndex)] = label ?? "";
    }

    public bool CurrentLineRequiresTrigger()
    {
        string segKey = NormalizeSegmentKey(GetCurrentSegmentName());
        return triggerRequiredLines.Contains((segKey, currentDialogueIndex));
    }

    public string GetCurrentTriggerLabel()
    {
        string segKey = NormalizeSegmentKey(GetCurrentSegmentName());
        if (triggerLabels.TryGetValue((segKey, currentDialogueIndex), out string label))
            return label;
        return "";
    }

    void ShowCurrentDialogue()
    {
        if (dialogueSegments == null || dialogueSegments.Count == 0) return;
        if (currentSegmentIndex < 0 || currentSegmentIndex >= dialogueSegments.Count) return;

        var segment = dialogueSegments[currentSegmentIndex];
        if (segment == null || segment.dialogueLines == null || segment.dialogueLines.Count == 0) return;
        if (currentDialogueIndex < 0 || currentDialogueIndex >= segment.dialogueLines.Count) return;

        StopTyping();
        StopVoice();

        string currentLine = segment.dialogueLines[currentDialogueIndex];

        typingCoroutine = StartCoroutine(TypeText(currentLine));

        if (prevButton != null) prevButton.interactable = currentDialogueIndex > 0;

        bool requiresTrigger = CurrentLineRequiresTrigger();
        bool hasNextLine = currentDialogueIndex < segment.dialogueLines.Count - 1;

        if (nextButton != null) nextButton.interactable = !requiresTrigger && hasNextLine;

        Debug.Log($"Showing dialogue [{GetCurrentSegmentName()}:{currentDialogueIndex}] — \"{currentLine}\"");

        PlayVoiceForCurrentLine();

        if (requiresTrigger)
        {
            string label = GetCurrentTriggerLabel();
            if (!string.IsNullOrEmpty(label))
                Debug.Log($"This line REQUIRES trigger to proceed: ({NormalizeSegmentKey(GetCurrentSegmentName())}:{currentDialogueIndex}) -> {label}");
            else
                Debug.Log($"This line REQUIRES trigger to proceed: ({NormalizeSegmentKey(GetCurrentSegmentName())}:{currentDialogueIndex})");
        }
    }

    void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        canProceed = true;
    }

    void StopVoice()
    {
        if (voiceSource != null && voiceSource.isPlaying)
        {
            voiceSource.Stop();
            voiceSource.clip = null;
        }
        SetTalking(false);
    }

    IEnumerator TypeText(string line)
    {
        canProceed = false;
        if (dialogueText != null) dialogueText.text = "";

        SetTalking(true);

        if (!string.IsNullOrEmpty(line))
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (dialogueText != null) dialogueText.text += line[i];
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        canProceed = true;

        // If trigger required, STOP here and wait for external trigger to call AdvanceDialogue()
        if (CurrentLineRequiresTrigger())
        {
            Debug.Log($"Dialogue paused — waiting for trigger ({NormalizeSegmentKey(GetCurrentSegmentName())}:{currentDialogueIndex})");
            yield break;
        }

        // otherwise auto-advance after voice finishes (safe even if voiceSource is null)
        yield return new WaitUntil(() => voiceSource == null || !voiceSource.isPlaying);

        SetTalking(false);
        yield return new WaitForSeconds(0.5f);
        AdvanceDialogue();
    }

    void PlayVoiceForCurrentLine()
    {
        if (voiceSource == null) return;
        if (cutsceneMode) return;

        AudioClip clip = GetAssignedVoiceClip(currentSegmentIndex, currentDialogueIndex);

        if (clip == null && !string.IsNullOrWhiteSpace(audioFolderPath))
        {
            string segKey = NormalizeSegmentKey(GetCurrentSegmentName());
            string fileName = $"{segKey}_{currentDialogueIndex:D2}";
            string fullPath = Path.Combine(audioFolderPath, fileName);

            clip = Resources.Load<AudioClip>(fullPath);
            if (clip != null)
                Debug.Log($"Playing voice clip (Resources): {fullPath}");
        }

        if (clip == null) return;

        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();

        SetTalking(true);
    }

    AudioClip GetAssignedVoiceClip(int segmentIndex, int lineIndex)
    {
        if (dialogueSegments == null) return null;
        if (segmentIndex < 0 || segmentIndex >= dialogueSegments.Count) return null;

        var seg = dialogueSegments[segmentIndex];
        if (seg == null || seg.voiceClips == null) return null;
        if (lineIndex < 0 || lineIndex >= seg.voiceClips.Count) return null;

        return seg.voiceClips[lineIndex];
    }

    void NextDialogue()
    {
        // Prevent manual skipping past trigger-gated lines
        if (CurrentLineRequiresTrigger())
        {
            Debug.Log($"Next blocked: trigger required ({NormalizeSegmentKey(GetCurrentSegmentName())}:{currentDialogueIndex}) -> {GetCurrentTriggerLabel()}");
            return;
        }

        var lines = dialogueSegments[currentSegmentIndex].dialogueLines;
        if (lines == null) return;

        if (currentDialogueIndex < lines.Count - 1)
        {
            currentDialogueIndex++;
            ShowCurrentDialogue();
        }
        else
        {
            Debug.Log($"Segment completed: {GetCurrentSegmentName()}");
            MoveToNextSegment();
        }
    }

    void PrevDialogue()
    {
        if (currentDialogueIndex > 0)
        {
            currentDialogueIndex--;
            ShowCurrentDialogue();
        }
    }

    void SkipDialogue()
    {
        // For safety, block if current line requires a trigger.
        if (CurrentLineRequiresTrigger())
        {
            Debug.Log($"Skip blocked: trigger required ({NormalizeSegmentKey(GetCurrentSegmentName())}:{currentDialogueIndex}) -> {GetCurrentTriggerLabel()}");
            return;
        }

        var lines = dialogueSegments[currentSegmentIndex].dialogueLines;
        if (lines == null || lines.Count == 0) return;

        currentDialogueIndex = lines.Count - 1;
        ShowCurrentDialogue();
    }

    public void SetDialogueSegment(int index)
    {
        if (index >= 0 && index < dialogueSegments.Count)
        {
            currentSegmentIndex = index;
            currentDialogueIndex = 0;
            ShowCurrentDialogue();
        }
    }

    /// <summary>
    /// Call this from your trigger scripts when the user completes the required action.
    /// Example: WDialogueManager.Instance.AdvanceDialogue();
    /// </summary>
    public void AdvanceDialogue()
    {
        Debug.Log($"Advancing dialogue externally ({GetCurrentSegmentName()}:{currentDialogueIndex})");

        SetTalking(false);

        var seg = dialogueSegments[currentSegmentIndex];
        bool hasNextLine = currentDialogueIndex < seg.dialogueLines.Count - 1;

        if (nextButton != null)
            nextButton.interactable = hasNextLine && !CurrentLineRequiresTrigger();

        NextDialogue();
    }

    private void MoveToNextSegment()
    {
        StopTyping();
        StopVoice();

        if (currentSegmentIndex < dialogueSegments.Count - 1)
        {
            currentSegmentIndex++;
            currentDialogueIndex = 0;
            Debug.Log($"Moving to next segment: {GetCurrentSegmentName()}");
            ShowCurrentDialogue();
        }
        else
        {
            Debug.Log("All dialogue segments completed!");
            SetTalking(false);
        }
    }

    public string GetCurrentSegmentName()
    {
        if (dialogueSegments == null || dialogueSegments.Count == 0) return "";
        if (currentSegmentIndex < 0 || currentSegmentIndex >= dialogueSegments.Count) return "";
        return dialogueSegments[currentSegmentIndex].segmentName;
    }

    public int GetCurrentLineIndex()
    {
        return currentDialogueIndex;
    }

    public void ShowCutsceneLine(int lineIndex)
    {
        if (!cutsceneMode) return;
        currentSegmentIndex = 0;
        currentDialogueIndex = lineIndex;
        ShowCurrentDialogue();
    }
}
