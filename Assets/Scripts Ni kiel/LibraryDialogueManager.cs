using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LibraryDialogueManager : MonoBehaviour
{
    public static LibraryDialogueManager Instance { get; private set; }

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

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.03f;

    [Header("Cutscene Auto Advance")]
    public bool autoAdvance = true;     // ✅ turn ON for cutscenes
    public float lineEndDelay = 0.25f;  // ✅ small pause after voice/text ends

    private bool isTyping = false;

    [System.Serializable]
    public class DialogueSegment
    {
        public string segmentName;

        [TextArea(3, 10)]
        public List<string> dialogueLines = new List<string>();

        public List<AudioClip> voiceClips = new List<AudioClip>();
    }

    [Header("NPC Animation (Optional)")]
    public Animator npcAnimator;
    public string isTalkingParam = "IsTalking";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple LibraryDialogueManager instances found. Keeping the first.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (nextButton != null) nextButton.onClick.AddListener(NextDialogue);
        if (prevButton != null) prevButton.onClick.AddListener(PrevDialogue);
        if (skipButton != null) skipButton.onClick.AddListener(SkipDialogue);

        EnsureVoiceClipListSizes();

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

    private void EnsureVoiceClipListSizes()
    {
        if (dialogueSegments == null) return;

        foreach (var seg in dialogueSegments)
        {
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
    }

    private IEnumerator TypeText(string line)
    {
        isTyping = true;

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

        isTyping = false;

        // ✅ Wait for voice to finish (if any)
        yield return new WaitUntil(() => voiceSource == null || !voiceSource.isPlaying);

        SetTalking(false);

        // ✅ Small pause so it feels natural
        if (lineEndDelay > 0f)
            yield return new WaitForSeconds(lineEndDelay);

        // ✅ AUTO ADVANCE for cutscene
        if (autoAdvance)
            AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
        NextDialogue();
    }

    private void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        isTyping = false;
    }

    private void StopVoice()
    {
        if (voiceSource != null && voiceSource.isPlaying)
            voiceSource.Stop();
    }

    private void PlayVoiceForCurrentLine()
    {
        if (voiceSource == null) return;

        var segment = dialogueSegments[currentSegmentIndex];
        if (segment == null || segment.voiceClips == null) return;

        if (currentDialogueIndex < 0 || currentDialogueIndex >= segment.voiceClips.Count)
            return;

        AudioClip clip = segment.voiceClips[currentDialogueIndex];
        if (clip == null) return;

        voiceSource.spatialBlend = 0f;
        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    private void NextDialogue()
    {
        if (dialogueSegments == null || dialogueSegments.Count == 0) return;

        // ✅ If user clicks Next while typing, instantly finish the line instead of skipping it.
        if (isTyping)
        {
            StopTyping();

            var seg = dialogueSegments[currentSegmentIndex];
            if (dialogueText != null)
                dialogueText.text = seg.dialogueLines[currentDialogueIndex];

            // Keep voice playing; cutscene can continue when voice ends
            // If you prefer skipping voice too, uncomment:
            // StopVoice();

            // Now continue auto-advance after voice ends:
            typingCoroutine = StartCoroutine(WaitThenAutoAdvance());
            return;
        }

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

    private IEnumerator WaitThenAutoAdvance()
    {
        yield return new WaitUntil(() => voiceSource == null || !voiceSource.isPlaying);

        SetTalking(false);

        if (lineEndDelay > 0f)
            yield return new WaitForSeconds(lineEndDelay);

        if (autoAdvance)
            AdvanceDialogue();
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

        StopTyping();
        StopVoice();

        var segment = dialogueSegments[currentSegmentIndex];
        if (segment == null || segment.dialogueLines == null || segment.dialogueLines.Count == 0) return;

        currentDialogueIndex = segment.dialogueLines.Count - 1;
        ShowCurrentDialogue();
    }

    private void MoveToNextSegment()
    {
        StopTyping();
        StopVoice();

        if (currentSegmentIndex < dialogueSegments.Count - 1)
        {
            currentSegmentIndex++;
            currentDialogueIndex = 0;
            ShowCurrentDialogue();
        }
        else
        {
            SetTalking(false);
            Debug.Log("Library cutscene dialogue finished.");
        }
    }

    public string GetCurrentSegmentName()
    {
        if (dialogueSegments == null || dialogueSegments.Count == 0) return "";
        return dialogueSegments[currentSegmentIndex].segmentName;
    }

    public int GetCurrentLineIndex()
    {
        return currentDialogueIndex;
    }
}
