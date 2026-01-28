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
            Debug.LogWarning("Multiple DialogueManager instances found. Keeping the first instance.");
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

        RegisterTriggerRequiredLines();
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
        // Lowercase + trim + remove spaces for stable matching across scripts, audio keys, and triggers.
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

    void RegisterTriggerRequiredLines()
    {
        triggerRequiredLines.Clear();

        // Use normalized keys here (spaces removed) to match NormalizeSegmentKey behavior.
        triggerRequiredLines.Add(("handwashing", 2));
        triggerRequiredLines.Add(("handwashing", 3));
        triggerRequiredLines.Add(("handwashing", 4));
        triggerRequiredLines.Add(("handwashing", 5));
        triggerRequiredLines.Add(("handwashing", 6));
        triggerRequiredLines.Add(("handwashing", 7));

        triggerRequiredLines.Add(("glovescoat", 0));
        triggerRequiredLines.Add(("glovescoat", 1));

        triggerRequiredLines.Add(("vaccineprep", 0));
        triggerRequiredLines.Add(("vaccineprep", 3));
        triggerRequiredLines.Add(("vaccineprep", 4));
        triggerRequiredLines.Add(("vaccineprep", 6));
        triggerRequiredLines.Add(("vaccineprep", 7));
        triggerRequiredLines.Add(("vaccineprep", 9));
        triggerRequiredLines.Add(("vaccineprep", 10));

        triggerRequiredLines.Add(("injection", 3));
        triggerRequiredLines.Add(("injection", 5));
        triggerRequiredLines.Add(("injection", 7));
        triggerRequiredLines.Add(("injection", 9));
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
        if (nextButton != null) nextButton.interactable = currentDialogueIndex < segment.dialogueLines.Count - 1;

        Debug.Log($"Showing dialogue [{GetCurrentSegmentName()}:{currentDialogueIndex}] — \"{currentLine}\"");

        PlayVoiceForCurrentLine();
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

        string segKey = NormalizeSegmentKey(GetCurrentSegmentName());
        bool requiresTrigger = triggerRequiredLines.Contains((segKey, currentDialogueIndex));

        if (requiresTrigger)
        {
            Debug.Log($"Dialogue paused — waiting for trigger ({segKey}:{currentDialogueIndex})");
            yield break;
        }

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

    public void AdvanceDialogue()
    {
        Debug.Log($"Advancing dialogue externally ({GetCurrentSegmentName()}:{currentDialogueIndex})");

        string segmentKey = NormalizeSegmentKey(GetCurrentSegmentName());
        int line = currentDialogueIndex;

        if (ChecklistManager.Instance != null)
        {
            if (segmentKey == "handwashing" && line == 5)
                DialogueChecklist.Instance?.CompleteTask("Sanitize");

            if (segmentKey == "glovescoat" && line == 1)
                DialogueChecklist.Instance?.CompleteTask("Wear PPE");

            if (segmentKey == "vaccineprep" && line == 10)
                DialogueChecklist.Instance?.CompleteTask("Prepare Vaccine");

            if (segmentKey == "injection" && line == 9)
                DialogueChecklist.Instance?.CompleteTask("Vaccinate the dog");
        }

        SetTalking(false);
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
