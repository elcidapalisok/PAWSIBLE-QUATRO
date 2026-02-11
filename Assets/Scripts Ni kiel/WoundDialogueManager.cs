using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    [Header("Audio Settings")]
    public AudioSource voiceSource;

    [Tooltip("Not used unless you implement Resources.Load audio auto-loading.")]
    public string audioFolderPath = "Audio/Vaccination_StoryMode";

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.03f;

    // These are the lines that must be advanced by triggers (faucet/soap/sanitizer/etc.)
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

    // ✅ Checklist task names (MUST match your checklist taskName exactly)
    private const string TASK_SANITIZE = "Sanitize";   // ✅ FIXED
    private const string TASK_PPE = "Wear PPE";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple WoundDialogueManager instances found. Keeping the first instance.");
            Destroy(gameObject);
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

    private void OnValidate()
    {
        EnsureVoiceClipListSizes();
    }

    private void SetTalking(bool talking)
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

    private void EnsureVoiceClipListSizes()
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

    // ✅ Add any “pause until trigger” lines here.
    private void RegisterTriggerRequiredLines()
    {
        triggerRequiredLines.Clear();

        triggerRequiredLines.Add(("handwashing", 2));
        triggerRequiredLines.Add(("handwashing", 3));
        triggerRequiredLines.Add(("handwashing", 4));
        triggerRequiredLines.Add(("handwashing", 5));
        triggerRequiredLines.Add(("handwashing", 6));
        triggerRequiredLines.Add(("handwashing", 7));

        triggerRequiredLines.Add(("glovescoat", 0));
        triggerRequiredLines.Add(("glovescoat", 1));
    }

    private void ShowCurrentDialogue()
    {
        if (dialogueSegments == null || dialogueSegments.Count == 0) return;
        if (currentSegmentIndex < 0 || currentSegmentIndex >= dialogueSegments.Count) return;

        var segment = dialogueSegments[currentSegmentIndex];
        if (segment == null || segment.dialogueLines == null || segment.dialogueLines.Count == 0) return;
        if (currentDialogueIndex < 0 || currentDialogueIndex >= segment.dialogueLines.Count) return;

        StopTyping();
        StopVoice();

        string line = segment.dialogueLines[currentDialogueIndex];

        typingCoroutine = StartCoroutine(TypeText(line));
        PlayVoiceForCurrentLine();

        if (prevButton) prevButton.interactable = currentDialogueIndex > 0;
        if (nextButton) nextButton.interactable = currentDialogueIndex < segment.dialogueLines.Count - 1;

        Debug.Log($"📢 Showing [{segment.segmentName}:{currentDialogueIndex}] {line}");
    }

    private IEnumerator TypeText(string line)
    {
        if (dialogueText != null)
            dialogueText.text = "";

        SetTalking(true);

        if (!string.IsNullOrEmpty(line))
        {
            foreach (char c in line)
            {
                if (dialogueText != null)
                    dialogueText.text += c;

                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // ✅ If this line requires a trigger, STOP here and wait for AdvanceDialogue()
        string key = NormalizeSegmentKey(GetCurrentSegmentName());
        if (triggerRequiredLines.Contains((key, currentDialogueIndex)))
            yield break;

        // ✅ Wait for voice to finish before auto-advance
        yield return new WaitUntil(() => voiceSource == null || !voiceSource.isPlaying);

        SetTalking(false);
        yield return new WaitForSeconds(0.25f);

        AdvanceDialogue();
    }

    private void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    private void StopVoice()
    {
        if (voiceSource != null && voiceSource.isPlaying)
            voiceSource.Stop();
    }

    private void PlayVoiceForCurrentLine()
    {
        if (voiceSource == null)
        {
            Debug.LogWarning("⚠️ VoiceSource is NULL. Assign an AudioSource in the Inspector.");
            return;
        }

        if (dialogueSegments == null || dialogueSegments.Count == 0) return;

        var segment = dialogueSegments[currentSegmentIndex];
        if (segment == null || segment.voiceClips == null) return;

        if (currentDialogueIndex < 0 || currentDialogueIndex >= segment.voiceClips.Count)
        {
            Debug.LogWarning($"⚠️ Voice clip index out of range: {currentDialogueIndex}/{segment.voiceClips.Count}");
            return;
        }

        AudioClip clip = segment.voiceClips[currentDialogueIndex];
        if (clip == null)
        {
            Debug.LogWarning($"⚠️ No voice clip assigned for [{segment.segmentName}:{currentDialogueIndex}]");
            return;
        }

        voiceSource.spatialBlend = 0f; // 2D voice
        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();

        Debug.Log($"🔊 Playing voice: {clip.name} for [{segment.segmentName}:{currentDialogueIndex}]");
    }

    private void NextDialogue()
    {
        if (dialogueSegments == null || dialogueSegments.Count == 0) return;

        var segment = dialogueSegments[currentSegmentIndex];
        if (segment == null || segment.dialogueLines == null) return;

        if (currentDialogueIndex < segment.dialogueLines.Count - 1)
        {
            currentDialogueIndex++;
            ShowCurrentDialogue();
            return;
        }

        MoveToNextSegment();
    }

    private void PrevDialogue()
    {
        if (dialogueSegments == null || dialogueSegments.Count == 0) return;

        if (currentDialogueIndex > 0)
        {
            currentDialogueIndex--;
            ShowCurrentDialogue();
        }
    }

    private void SkipDialogue()
    {
        if (dialogueSegments == null || dialogueSegments.Count == 0) return;

        var segment = dialogueSegments[currentSegmentIndex];
        if (segment == null || segment.dialogueLines == null || segment.dialogueLines.Count == 0) return;

        currentDialogueIndex = segment.dialogueLines.Count - 1;
        ShowCurrentDialogue();
    }

    public void AdvanceDialogue()
    {
        NextDialogue();
    }

    private void MoveToNextSegment()
    {
        StopTyping();
        StopVoice();

        if (dialogueSegments == null || dialogueSegments.Count == 0) return;

        string finishedSegment = NormalizeSegmentKey(GetCurrentSegmentName());
        Debug.Log($"✅ Finished segment = '{finishedSegment}'");

        if (WoundDialogueChecklist.Instance == null)
        {
            Debug.LogWarning("⚠️ WoundDialogueChecklist.Instance is NULL. Checklist object missing or disabled in scene.");
        }
        else
        {
            if (finishedSegment == "handwashing")
            {
                Debug.Log($"🧾 Completing checklist task: {TASK_SANITIZE}");
                WoundDialogueChecklist.Instance.CompleteTask(TASK_SANITIZE);
            }
            else if (finishedSegment == "glovescoat")
            {
                Debug.Log($"🧾 Completing checklist task: {TASK_PPE}");
                WoundDialogueChecklist.Instance.CompleteTask(TASK_PPE);
            }
        }

        if (currentSegmentIndex < dialogueSegments.Count - 1)
        {
            currentSegmentIndex++;
            currentDialogueIndex = 0;

            Debug.Log($"➡ Moving to next segment: {GetCurrentSegmentName()}");
            ShowCurrentDialogue();
        }
        else
        {
            Debug.Log("✅ All Wound dialogue segments completed!");
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
}
