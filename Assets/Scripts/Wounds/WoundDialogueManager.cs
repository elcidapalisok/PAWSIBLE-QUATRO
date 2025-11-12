using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class WoundDialogueManager : MonoBehaviour
{
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
    [Tooltip("Relative path inside Resources folder (e.g. 'Audio/WoundTreatment')")]
    public string audioFolderPath = "Audio/WoundTreatment";

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.03f;

    // Tracks which lines require a trigger to advance
    private HashSet<(string, int)> triggerRequiredLines = new HashSet<(string, int)>();

    [System.Serializable]
    public class DialogueSegment
    {
        public string segmentName; // e.g. "Sanitize and PPE"
        [TextArea(3, 10)]
        public List<string> dialogueLines = new List<string>();
    }

    [Header("Cutscene Control")]
    public bool cutsceneMode = false; // If true, audio auto-play is skipped

    void Start()
    {
        if (nextButton != null) nextButton.onClick.AddListener(NextDialogue);
        if (prevButton != null) prevButton.onClick.AddListener(PrevDialogue);
        if (skipButton != null) skipButton.onClick.AddListener(SkipDialogue);

        RegisterTriggerRequiredLines();

        if (dialogueSegments.Count > 0)
            ShowCurrentDialogue();
    }

    // ----------------------------
    // REGISTER TRIGGERS
    // ----------------------------
    void RegisterTriggerRequiredLines()
    {
        // Sanitize and PPE segment
        triggerRequiredLines.Add(("Sanitize and PPE", 1));
        triggerRequiredLines.Add(("Sanitize and PPE", 2));
        triggerRequiredLines.Add(("Sanitize and PPE", 3));
        triggerRequiredLines.Add(("Sanitize and PPE", 4));
        triggerRequiredLines.Add(("Sanitize and PPE", 5));
        triggerRequiredLines.Add(("Sanitize and PPE", 6));

        // Scratch Examine segment
        triggerRequiredLines.Add(("Scratch Examine", 0));
        triggerRequiredLines.Add(("Scratch Examine", 2));
        triggerRequiredLines.Add(("Scratch Examine", 6));

        // Equipments segment
        triggerRequiredLines.Add(("Equipments", 0));
        triggerRequiredLines.Add(("Equipments", 1));
        triggerRequiredLines.Add(("Equipments", 2));

        // Treatment segment
        triggerRequiredLines.Add(("Treatment", 0));
        triggerRequiredLines.Add(("Treatment", 2));
        triggerRequiredLines.Add(("Treatment", 3));
        triggerRequiredLines.Add(("Treatment", 4));
        triggerRequiredLines.Add(("Treatment", 5));
        triggerRequiredLines.Add(("Treatment", 7));
        triggerRequiredLines.Add(("Treatment", 10));
    }

    // ----------------------------
    // MAIN DIALOGUE DISPLAY
    // ----------------------------
    void ShowCurrentDialogue()
    {
        if (dialogueSegments.Count == 0) return;

        var lines = dialogueSegments[currentSegmentIndex].dialogueLines;
        if (lines.Count == 0) return;

        string currentLine = lines[currentDialogueIndex];

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(currentLine));

        prevButton.interactable = currentDialogueIndex > 0;
        nextButton.interactable = currentDialogueIndex < lines.Count - 1;

        string segment = GetCurrentSegmentName();
        Debug.Log($"Showing dialogue [{segment}:{currentDialogueIndex}] — \"{currentLine}\"");

        PlayVoiceForCurrentLine();
    }

    // ----------------------------
    // TYPEWRITER EFFECT
    // ----------------------------
    IEnumerator TypeText(string line)
    {
        canProceed = false;
        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        canProceed = true;

        // Check if this line requires a trigger
        string segment = GetCurrentSegmentName();
        bool requiresTrigger = triggerRequiredLines.Contains((segment, currentDialogueIndex));

        if (requiresTrigger)
        {
            Debug.Log($"Dialogue paused — waiting for trigger ({segment}:{currentDialogueIndex})");
            yield break;
        }

        // Wait for audio to finish, then auto advance
        yield return new WaitUntil(() => voiceSource == null || !voiceSource.isPlaying);
        yield return new WaitForSeconds(0.5f);
        AdvanceDialogue();
    }

    // ----------------------------
    // AUDIO HANDLING
    // ----------------------------
    void PlayVoiceForCurrentLine()
    {
        if (voiceSource == null || cutsceneMode) return;

        string segment = GetCurrentSegmentName().Replace(" ", "");
        string fileName = $"{segment}_{currentDialogueIndex:D2}";
        string fullPath = Path.Combine(audioFolderPath, fileName);

        AudioClip clip = Resources.Load<AudioClip>(fullPath);
        if (clip != null)
        {
            voiceSource.clip = clip;
            voiceSource.Play();
            Debug.Log($"Playing voice clip: {fullPath}");
        }
        else
        {
            Debug.LogWarning($"Voice clip not found: {fullPath}");
        }
    }

    // ----------------------------
    // BUTTON HANDLERS
    // ----------------------------
    void NextDialogue()
    {
        var lines = dialogueSegments[currentSegmentIndex].dialogueLines;
        if (currentDialogueIndex < lines.Count - 1)
        {
            currentDialogueIndex++;
            ShowCurrentDialogue();
        }
        else
        {
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

        string segment = GetCurrentSegmentName();
        int line = currentDialogueIndex;

        // Complete relevant tasks if using checklist
        if (DialogueChecklist.Instance != null)
        {
            if (segment == "Sanitize and PPE")
            {
                if (line == 5) DialogueChecklist.Instance?.CompleteTask("Sanitize");
                if (line == 6) DialogueChecklist.Instance?.CompleteTask("Wear PPE");
            }

            if (segment == "Scratch Examine" && line == 6)
                DialogueChecklist.Instance?.CompleteTask("Check Equipment");

            if (segment == "Treatment")
            {
                if (line == 10) DialogueChecklist.Instance?.CompleteTask("Treatment Complete");
            }
        }

        NextDialogue();
    }

    private void MoveToNextSegment()
    {
        if (currentSegmentIndex < dialogueSegments.Count - 1)
        {
            currentSegmentIndex++;
            currentDialogueIndex = 0;
            ShowCurrentDialogue();
        }
        else
        {
            Debug.Log("All dialogue segments completed!");
        }
    }

    // ----------------------------
    // ACCESSORS
    // ----------------------------
    public string GetCurrentSegmentName()
    {
        return dialogueSegments[currentSegmentIndex].segmentName;
    }

    // Returns normalized segment name (no spaces, lowercase) for InteractableObject comparison
    public string GetCurrentSegmentNameNormalized()
    {
        return dialogueSegments[currentSegmentIndex].segmentName.Replace(" ", "").ToLower();
    }

    public int GetCurrentLineIndex()
    {
        return currentDialogueIndex;
    }

    // ----------------------------
    // CUTSCENE LINE FOR TIMELINE
    // ----------------------------
    public void ShowCutsceneLine(int lineIndex)
    {
        if (!cutsceneMode) return;

        currentSegmentIndex = 0; // assuming cutscene is segment 0
        currentDialogueIndex = lineIndex;
        ShowCurrentDialogue();
    }
}
