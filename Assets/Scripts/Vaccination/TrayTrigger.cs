using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrayTrigger : MonoBehaviour
{
    [Header("Required Vial Tags")]
    public string diluentVialTag = "DiluentVial";
    public string fiveInOneVialTag = "5in1";

    [Header("Dialogue Trigger Settings")]
    public string targetSegmentName = "Vaccine Prep";
    public int targetLineIndex = 3;

    [Header("Feedback Spawn")]
    public Transform feedbackAnchor;
    public Vector3 feedbackOffset = new Vector3(0f, 0.15f, 0f);

    [Header("Wrong Feedback")]
    public bool showWrongFeedbackWhenNotAtStage = true;
    [Min(0f)] public float wrongFeedbackCooldown = 1.0f;

    [Header("Behavior")]
    [Tooltip("Prevents retriggering until one of the required vials exits the tray.")]
    public bool blockUntilExit = true;

    private bool diluentPresent;
    private bool fiveInOnePresent;

    private bool blocked;

    [SerializeField] private DialogueManager dialogueManager;

    private float lastWrongTime = -999f;

    void Awake()
    {
        // Ensure trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (blocked) return;

        if (other.CompareTag(diluentVialTag))
        {
            diluentPresent = true;
            Debug.Log("Diluent vial placed on tray.");
        }

        if (other.CompareTag(fiveInOneVialTag))
        {
            fiveInOnePresent = true;
            Debug.Log("5-in-1 vial placed on tray.");
        }

        CheckForCompletion();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(diluentVialTag))
        {
            diluentPresent = false;
            Debug.Log("Diluent vial removed from tray.");
        }

        if (other.CompareTag(fiveInOneVialTag))
        {
            fiveInOnePresent = false;
            Debug.Log("5-in-1 vial removed from tray.");
        }

        // Unblock once either required vial leaves
        if (blockUntilExit)
        {
            if (!diluentPresent || !fiveInOnePresent)
                blocked = false;
        }
    }

    private void CheckForCompletion()
    {
        // Both vials must be present
        if (!diluentPresent || !fiveInOnePresent)
            return;

        if (dialogueManager == null)
            return;

        string currentSegment = DialogueManager.NormalizeSegmentKey(dialogueManager.GetCurrentSegmentName());
        string targetSegment = DialogueManager.NormalizeSegmentKey(targetSegmentName);
        int currentLine = dialogueManager.GetCurrentLineIndex();

        // Correct stage -> success
        if (currentSegment == targetSegment && currentLine == targetLineIndex)
        {
            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportCorrect(spawnPos);

            // SCORE: correct completion
            ScoreManager.Instance?.RegisterCorrect(targetSegmentName, targetLineIndex);

            Debug.Log("Both required vials placed — advancing dialogue.");

            if (blockUntilExit) blocked = true;

            dialogueManager.AdvanceDialogue();
            return;
        }

        // Wrong stage -> wrong feedback + mistake (optional)
        if (showWrongFeedbackWhenNotAtStage && Time.time - lastWrongTime >= wrongFeedbackCooldown)
        {
            lastWrongTime = Time.time;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportWrong(spawnPos);

            // SCORE: mistake registered at current stage
            ScoreManager.Instance?.RegisterMistake(
                dialogueManager.GetCurrentSegmentName(),
                dialogueManager.GetCurrentLineIndex(),
                "Placed required vials on tray at wrong stage"
            );
        }

        Debug.Log(
            $"Vials placed, but dialogue is at ({currentSegment}:{currentLine}) " +
            $"instead of ({targetSegment}:{targetLineIndex})"
        );

        if (blockUntilExit) blocked = true;
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        Transform anchor = feedbackAnchor != null ? feedbackAnchor : transform;
        return anchor.position + feedbackOffset;
    }
}
