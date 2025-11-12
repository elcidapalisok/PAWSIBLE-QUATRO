using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class BordetellaDialogueManager : MonoBehaviour
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
    [Tooltip("Relative path inside Resources folder (e.g. 'Audio/Bordetella')")]
    public string audioFolderPath = "Audio/Bordetella";

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.03f;

    private HashSet<(string, int)> triggerRequiredLines = new HashSet<(string, int)>();

    [System.Serializable]
    public class DialogueSegment
    {
        public string segmentName;
        [TextArea(3, 10)]
        public List<string> dialogueLines = new List<string>();
    }

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
        // Handwashing triggers
        triggerRequiredLines.Add(("handwashing", 2)); // Use soap
        triggerRequiredLines.Add(("handwashing", 3)); // Apply sanitizer

        // PPE triggers
        triggerRequiredLines.Add(("glovescoat", 0)); // Gloves
        triggerRequiredLines.Add(("glovescoat", 1)); // Lab coat

        // Vaccine prep triggers
        triggerRequiredLines.Add(("vaccine prep", 2)); // Pick up vial
        triggerRequiredLines.Add(("vaccine prep", 3)); // Draw liquid
        triggerRequiredLines.Add(("vaccine prep", 4)); // Replace needle

        // Injection triggers
        triggerRequiredLines.Add(("injection", 1)); // Temperature check
        triggerRequiredLines.Add(("injection", 3)); // Inject vaccine
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

        string segment = GetCurrentSegmentName().ToLower().Trim();
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
        if (voiceSource == null) return;

        string segment = GetCurrentSegmentName().Replace(" ", "").ToLower();
        string fileName = $"{segment}_{currentDialogueIndex:D2}";
        string fullPath = Path.Combine(audioFolderPath, fileName);

        AudioClip clip = Resources.Load<AudioClip>(fullPath);
        if (clip != null)
        {
            voiceSource.clip = clip;
            voiceSource.Play();
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
            Debug.Log("All Bordetella dialogue segments completed!");
        }
    }

    // ----------------------------
    // ACCESSORS
    // ----------------------------
    public string GetCurrentSegmentName()
    {
        return dialogueSegments[currentSegmentIndex].segmentName;
    }

    public int GetCurrentLineIndex()
    {
        return currentDialogueIndex;
    }
}
