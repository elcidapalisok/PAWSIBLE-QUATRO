using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StageBasedAreaWaypoint : MonoBehaviour
{
    [Header("Target Stage to Show Marker")]
    public string targetSegmentName = "VaccinePrep";
    public int targetLineIndex = 0;

    [Header("References")]
    public GameObject goalMarker; // Drag GoalMarker_Ring here

    [Header("Trigger Filtering")]
    [Tooltip("Set to the layer(s) used by your XR Rig / Player collider.")]
    public LayerMask playerLayers;

    [Tooltip("Optional tag check. Leave as 'Player' if you use Player tag; set empty to skip tag check.")]
    public string requiredPlayerTag = "Player";

    [Tooltip("If true: only allow triggering when currently at target segment+line.")]
    public bool requireExactStageToTrigger = true;

    [Header("Feedback Spawn")]
    public Transform feedbackAnchor;
    public Vector3 feedbackOffset = new Vector3(0f, 0.15f, 0f);

    [Header("Wrong Feedback")]
    public bool showWrongFeedbackWhenNotAtStage = true;
    [Min(0f)] public float wrongFeedbackCooldown = 1.0f;

    private bool markerShown;
    private bool fired;

    private float lastWrongTime = -999f;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Start()
    {
        if (goalMarker != null)
            goalMarker.SetActive(false); // always start hidden
    }

    void Update()
    {
        if (DialogueManager.Instance == null || goalMarker == null || fired)
            return;

        bool atStage = DialogueManager.Instance.IsAtStage(targetSegmentName, targetLineIndex);

        // Turn ON only when we reach the stage
        if (atStage && !markerShown)
        {
            goalMarker.SetActive(true);
            markerShown = true;
        }

        // Turn OFF if we leave the stage
        if (!atStage && markerShown)
        {
            goalMarker.SetActive(false);
            markerShown = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fired) return;

        // Layer filter
        if (!IsInLayerMask(other.gameObject.layer, playerLayers))
            return;

        // Optional Tag filter (useful if many things share the same layer)
        if (!string.IsNullOrWhiteSpace(requiredPlayerTag) && !other.CompareTag(requiredPlayerTag))
            return;

        if (DialogueManager.Instance == null)
            return;

        bool atStage = DialogueManager.Instance.IsAtStage(targetSegmentName, targetLineIndex);

        // Correct stage => success
        if (!requireExactStageToTrigger || atStage)
        {
            fired = true;

            // Feedback + score (correct)
            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportCorrect(spawnPos);
            ScoreManager.Instance?.RegisterCorrect(targetSegmentName, targetLineIndex);

            // Hide marker immediately
            if (goalMarker != null)
                goalMarker.SetActive(false);

            // Proceed dialogue
            DialogueManager.Instance.AdvanceDialogue();
            return;
        }

        // Wrong stage => wrong feedback + mistake (optional)
        if (showWrongFeedbackWhenNotAtStage && Time.time - lastWrongTime >= wrongFeedbackCooldown)
        {
            lastWrongTime = Time.time;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportWrong(spawnPos);

            ScoreManager.Instance?.RegisterMistake(
                DialogueManager.Instance.GetCurrentSegmentName(),
                DialogueManager.Instance.GetCurrentLineIndex(),
                "Entered waypoint area at wrong stage"
            );
        }
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        Transform anchor = feedbackAnchor != null ? feedbackAnchor : transform;
        return anchor.position + feedbackOffset;
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
