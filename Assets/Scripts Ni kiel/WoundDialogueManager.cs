using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class WoundDialogueManager : MonoBehaviour
{
    public static WoundDialogueManager Instance { get; private set; }

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

    [Tooltip("Optional fallback: Relative path inside Resources folder (e.g. 'Audio/Vaccination_StoryMode')")]
    public string audioFolderPath = "Audio/Vaccination_StoryMode";

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.03f;

    private HashSet<(string, int)> triggerRequiredLines = new HashSet<(string, int)>();

    [System.Serializable]
    public class DialogueSegment
    {
        public string segmentName;

        [TextArea(3, 10)]
        public List<string> dialogueLines = new List<string>();

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
            Debug.LogWarning("Multiple WoundDialogueManager instances found. Keeping the first instance.");
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("✅ WoundDialogueManager START running");

        if (nextButton != null) nextButton.onClick.AddListener(NextDialogue);
        if (prevButton != null) prevButton.onClick.AddListener(PrevDialogue);
        if (skipButton != null) skipButton.onClick.AddListener(SkipDialogue);

        RegisterTriggerRequiredLines();
        EnsureVoiceClipListSizes();

        Debug.Log("Segments count: " + (dialogueSegments == null ? -1 : dialogueSegments.Count));

        if (dialogueSegments != null && dialogueSegments.Count > 0)
            ShowCurrentDialogue();
    }

    void OnValidate()
    {
        EnsureVoiceClipListSizes();
    }

    void SetTalking(bool talking)
    {
        if (npcAnimator == null) return;
        npcAnimator.SetBool(isTalkingParam, talking);
    }

    public static string NormalizeSegmentKey(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
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

        foreach (var seg in dialogueSegments)
        {
            if (seg == null) continue;

            if (seg.dialogueLines == null)
                seg.dialogueLines = new List<string>();

            if (seg.voiceClips == null)
                seg.voiceClips = new List<AudioClip>();

            while (seg.voiceClips.Count < seg.dialogueLines.Count)
                seg.voiceClips.Add(null);

            if (seg.voiceClips.Count > seg.dialogueLines.Count)
                seg.voiceClips.RemoveRange(seg.dialogueLines.Count, seg.voiceClips.Count - seg.dialogueLines.Count);
        }
    }

    void RegisterTriggerRequiredLines()
    {
        triggerRequiredLines.Clear();

        triggerRequiredLines.Add(("handwashing", 2));
        triggerRequiredLines.Add(("handwashing", 3));
        triggerRequiredLines.Add(("handwashing", 4));
        triggerRequiredLines.Add(("handwashing", 5));
        triggerRequiredLines.Add(("handwashing", 6));
        triggerRequiredLines.Add(("handwashing", 7));
    }

    void ShowCurrentDialogue()
    {
        if (dialogueSegments.Count == 0) return;

        var segment = dialogueSegments[currentSegmentIndex];
        if (segment.dialogueLines.Count == 0) return;

        StopTyping();
        StopVoice();

        string line = segment.dialogueLines[currentDialogueIndex];
        typingCoroutine = StartCoroutine(TypeText(line));

        if (prevButton) prevButton.interactable = currentDialogueIndex > 0;
        if (nextButton) nextButton.interactable = currentDialogueIndex < segment.dialogueLines.Count - 1;

        Debug.Log($"📢 Showing [{segment.segmentName}:{currentDialogueIndex}] {line}");
    }

    IEnumerator TypeText(string line)
    {
        canProceed = false;
        dialogueText.text = "";

        SetTalking(true);

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        canProceed = true;

        string key = NormalizeSegmentKey(GetCurrentSegmentName());
        if (triggerRequiredLines.Contains((key, currentDialogueIndex)))
            yield break;

        yield return new WaitUntil(() => voiceSource == null || !voiceSource.isPlaying);

        SetTalking(false);
        yield return new WaitForSeconds(0.5f);

        AdvanceDialogue();
    }

    void StopTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
    }

    void StopVoice()
    {
        if (voiceSource != null && voiceSource.isPlaying)
            voiceSource.Stop();
    }

    void NextDialogue()
    {
        var lines = dialogueSegments[currentSegmentIndex].dialogueLines;

        if (currentDialogueIndex < lines.Count - 1)
        {
            currentDialogueIndex++;
            ShowCurrentDialogue();
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
        var lines = dialogueSegments[currentSegmentIndex].dialogueLines;
        currentDialogueIndex = lines.Count - 1;
        ShowCurrentDialogue();
    }

    public void AdvanceDialogue()
    {
        NextDialogue();
    }

    public string GetCurrentSegmentName()
    {
        return dialogueSegments[currentSegmentIndex].segmentName;
    }

    public int GetCurrentLineIndex()
    {
        return currentDialogueIndex;
    }
}
