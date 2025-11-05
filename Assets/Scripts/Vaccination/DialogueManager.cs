    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using UnityEngine.UI;
    using System.IO;

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

        [Header("Audio Settings")]
        public AudioSource voiceSource; // Assign this in Inspector
        [Tooltip("Relative path inside Resources folder (e.g. 'Audio/Vaccination_StoryMode')")]
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

        // ----------------------------
        // REGISTER TRIGGERS
        // ----------------------------
        void RegisterTriggerRequiredLines()
        {
            triggerRequiredLines.Add(("handwashing", 2));
            triggerRequiredLines.Add(("handwashing", 3));
            triggerRequiredLines.Add(("handwashing", 5));

            triggerRequiredLines.Add(("glovescoat", 0));
            triggerRequiredLines.Add(("glovescoat", 1));

            triggerRequiredLines.Add(("vaccine prep", 2));
            triggerRequiredLines.Add(("vaccine prep", 3));
            triggerRequiredLines.Add(("vaccine prep", 4));

            triggerRequiredLines.Add(("injection", 1));
            triggerRequiredLines.Add(("injection", 3));
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

            string segment = GetCurrentSegmentName().ToLower().Trim();
            bool requiresTrigger = triggerRequiredLines.Contains((segment, currentDialogueIndex));

            if (requiresTrigger)
            {
                Debug.Log($"Dialogue paused — waiting for trigger ({segment}:{currentDialogueIndex})");
                yield break;
            }

            // Wait for voice to finish, then auto advance
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
            string fileName = $"{segment}_{currentDialogueIndex:D2}"; // e.g. handwashing_00
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

        string segment = GetCurrentSegmentName().ToLower().Trim();
        int line = currentDialogueIndex;

        if (ChecklistManager.Instance != null)
        {
            if (segment == "handwashing" && line == 5)
                //ChecklistManager.Instance.CompleteTask("Sanitize");
                DialogueChecklist.Instance?.CompleteTask("Sanitize");

            if (segment == "glovescoat" && line == 2)
                //ChecklistManager.Instance.CompleteTask("Wear PPE");
                DialogueChecklist.Instance?.CompleteTask("Wear PPE");

            if (segment == "vaccine prep" && line == 6)
                //ChecklistManager.Instance.CompleteTask("Prepare Vaccine");
                DialogueChecklist.Instance?.CompleteTask("Prepare Vaccine");

            if (segment == "injection" && line == 4)
                //ChecklistManager.Instance.CompleteTask("Vaccinate the dog");
                DialogueChecklist.Instance?.CompleteTask("Vaccinate the dog");

        }

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
