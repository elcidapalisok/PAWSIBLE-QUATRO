using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
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
        nextButton.onClick.AddListener(NextDialogue);
        prevButton.onClick.AddListener(PrevDialogue);
        skipButton.onClick.AddListener(SkipDialogue);

        RegisterTriggerRequiredLines();

        if (dialogueSegments.Count > 0)
            ShowCurrentDialogue();
    }

    void RegisterTriggerRequiredLines()
    {
        // Handwashing
        triggerRequiredLines.Add(("handwashing", 2));
        triggerRequiredLines.Add(("handwashing", 3));
        triggerRequiredLines.Add(("handwashing", 5));

        // Gloves & Coat
        triggerRequiredLines.Add(("glovescoat", 0));
        triggerRequiredLines.Add(("glovescoat", 1));

        // Vaccine Prep
        triggerRequiredLines.Add(("vaccine prep", 2));
        triggerRequiredLines.Add(("vaccine prep", 3));
        triggerRequiredLines.Add(("vaccine prep", 4));

        // Injection
        triggerRequiredLines.Add(("injection", 1));
        triggerRequiredLines.Add(("injection", 3));
    }

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
    }

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

        string segment = GetCurrentSegmentName();
        string key = segment.ToLower().Trim();
        bool requiresTrigger = triggerRequiredLines.Contains((key, currentDialogueIndex));

        if (requiresTrigger)
        {
            Debug.Log($"Dialogue paused — waiting for trigger ({segment}:{currentDialogueIndex})");
            yield break; // stop here until object triggers AdvanceDialogue()
        }

        // Auto advance if no trigger needed
        yield return new WaitForSeconds(1f);
        AdvanceDialogue();
    }

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
            // Segment complete
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
        NextDialogue();
    }

    private void MoveToNextSegment()
    {
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
        }
    }

    // Accessors
    public string GetCurrentSegmentName()
    {
        return dialogueSegments[currentSegmentIndex].segmentName;
    }

    public int GetCurrentLineIndex()
    {
        return currentDialogueIndex;
    }
}
