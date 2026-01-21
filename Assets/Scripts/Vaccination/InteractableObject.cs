using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string objectName = "Interactable";
    public bool advanceDialogueOnUse = true;

    [Header("Dialogue Trigger Target")]
    public string targetSegmentName;
    public int targetLineIndex;

    [Header("Feedback")]
    public bool showWrongFeedbackWhenNotAtStage = true;
    [Min(0f)] public float wrongFeedbackCooldown = 1.0f;

    [Tooltip("If assigned, feedback spawns at this transform. Otherwise uses this object's transform.")]
    public Transform feedbackAnchor;

    [Tooltip("Offset added to feedback spawn position.")]
    public Vector3 feedbackOffset = new Vector3(0f, 0.15f, 0f);

    [Header("Timed Hold Requirement")]
    [Tooltip("Enable to require holding/selecting the object for a duration before advancing dialogue.")]
    public bool useHoldTimer = false;

    [Min(0.1f)]
    public float holdDurationSeconds = 6f;

    [Tooltip("If true, releasing the object resets hold progress. If false, progress is kept but only counts while held.")]
    public bool resetTimerOnRelease = true;

    [Tooltip("If true, timer counts only when the user is currently selecting/holding the object.")]
    public bool timerCountsOnlyWhileSelected = true;

    [Header("References (Optional)")]
    [SerializeField] private DialogueManager dialogueManager;

    private XRBaseInteractable interactable;
    private float lastWrongTime = -999f;

    private bool isSelected = false;
    private float holdTimer = 0f;
    private bool completedThisStage = false;

    private void Awake()
    {
        if (string.IsNullOrEmpty(objectName))
            objectName = gameObject.name;

        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;

        interactable = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void Update()
    {
        if (!useHoldTimer) return;
        if (completedThisStage) return;
        if (dialogueManager == null) return;

        // Timer counts only while selected (recommended default)
        if (timerCountsOnlyWhileSelected && !isSelected)
            return;

        // Only count time at the correct stage
        if (!dialogueManager.IsAtStage(targetSegmentName, targetLineIndex))
        {
            holdTimer = 0f;
            return;
        }

        // Count time
        holdTimer += Time.deltaTime;

        if (holdTimer >= holdDurationSeconds)
        {
            CompleteCorrect();
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        isSelected = true;

        if (dialogueManager == null) return;

        bool atStage = dialogueManager.IsAtStage(targetSegmentName, targetLineIndex);

        // Instant mode (default behavior)
        if (!useHoldTimer)
        {
            if (atStage)
            {
                CompleteCorrect();
            }
            else
            {
                MaybeWrongFeedback();
            }
            return;
        }

        // Timed mode
        if (!atStage)
        {
            // Wrong stage: show wrong feedback, but do not advance
            MaybeWrongFeedback();

            // Optional: prevent "pre-holding" from counting
            holdTimer = 0f;
            return;
        }

        // At correct stage:
        // If you want timer to start fresh every time it is selected, reset here:
        if (resetTimerOnRelease)
            holdTimer = 0f;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        isSelected = false;

        if (!useHoldTimer) return;
        if (!resetTimerOnRelease) return;

        holdTimer = 0f;
        completedThisStage = false;
    }

    private void CompleteCorrect()
    {
        if (completedThisStage) return;

        completedThisStage = true;
        holdTimer = 0f;

        // --- SCORE: correct completion of this step ---
        ScoreManager.Instance?.RegisterCorrect(targetSegmentName, targetLineIndex);

        Vector3 spawnPos = GetFeedbackSpawnPosition();
        FeedbackManager.Instance?.ReportCorrect(spawnPos);

        if (advanceDialogueOnUse && dialogueManager != null)
        {
            dialogueManager.AdvanceDialogue();
        }

        Debug.Log($"{objectName} triggered dialogue (Correct) at {targetSegmentName}:{targetLineIndex}");
    }

    private void MaybeWrongFeedback()
    {
        if (!showWrongFeedbackWhenNotAtStage) return;

        if (Time.time - lastWrongTime >= wrongFeedbackCooldown)
        {
            lastWrongTime = Time.time;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportWrong(spawnPos);

            // --- SCORE: mistake registered on actual current stage ---
            if (dialogueManager != null)
            {
                ScoreManager.Instance?.RegisterMistake(
                    dialogueManager.GetCurrentSegmentName(),
                    dialogueManager.GetCurrentLineIndex(),
                    $"{objectName} used at wrong stage"
                );

                Debug.Log($"{objectName} used at wrong stage (current {DialogueManager.NormalizeSegmentKey(dialogueManager.GetCurrentSegmentName())}:{dialogueManager.GetCurrentLineIndex()})");
            }
        }
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        Transform anchor = feedbackAnchor != null ? feedbackAnchor : transform;
        return anchor.position + feedbackOffset;
    }
}
