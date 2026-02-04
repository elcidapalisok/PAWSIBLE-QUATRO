using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VialNeedleAdvanceTrigger : MonoBehaviour
{
    [Header("Dialogue Gate")]
    public string targetSegmentName = "Vaccine Prep";
    public int targetLineIndex = 4;

    [Header("Needle Detection")]
    [Tooltip("Tag used on the needle tip collider (recommended).")]
    public string needleTipTag = "NeedleTip";

    [Header("Feedback Spawn")]
    public Transform feedbackAnchor;
    public Vector3 feedbackOffset = new Vector3(0f, 0.15f, 0f);

    [Header("Wrong Feedback")]
    public bool showWrongFeedbackWhenNotAtStage = true;
    [Min(0f)] public float wrongFeedbackCooldown = 1.0f;

    [Header("Behavior")]
    public bool triggerOnce = true;

    [Header("Dependencies (optional)")]
    [SerializeField] private DialogueManager dialogueManager;

    private bool fired;
    private float lastWrongTime = -999f;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Awake()
    {
        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && fired) return;
        if (dialogueManager == null) return;

        // Only react to the needle tip
        if (!other.CompareTag(needleTipTag))
            return;

        // Success path: correct stage + trigger met
        if (dialogueManager.IsAtStage(targetSegmentName, targetLineIndex))
        {
            fired = true;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportCorrect(spawnPos);

            // SCORE: correct completion of this stage
            ScoreManager.Instance?.RegisterCorrect(targetSegmentName, targetLineIndex);

            dialogueManager.AdvanceDialogue();
            return;
        }

        // Wrong-stage path (optional)
        if (showWrongFeedbackWhenNotAtStage && Time.time - lastWrongTime >= wrongFeedbackCooldown)
        {
            lastWrongTime = Time.time;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportWrong(spawnPos);

            // SCORE: mistake registered on actual current stage
            ScoreManager.Instance?.RegisterMistake(
                dialogueManager.GetCurrentSegmentName(),
                dialogueManager.GetCurrentLineIndex(),
                "Needle entered vial trigger at wrong stage"
            );
        }
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        Transform anchor = feedbackAnchor != null ? feedbackAnchor : transform;
        return anchor.position + feedbackOffset;
    }
}
