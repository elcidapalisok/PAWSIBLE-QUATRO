using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WMultiStageTimedDialogueTriggerZone : MonoBehaviour
{
    [System.Serializable]
    public class StageGate
    {
        [Tooltip("Segment name to match (Inspector manual). Example: Handwashing, WoundStabilizationPrep, WoundStabilization_Cleaning")]
        public string segmentName;

        [Tooltip("Exact line index to match within the segment (0-based)")]
        public int lineIndex;
    }

    [Header("Allowed Dialogue Stages (Any of these can complete the timer)")]
    public List<StageGate> allowedStages = new List<StageGate>();

    [Header("Matching Mode")]
    [Tooltip("ON = ignores casing/spaces differences (recommended). OFF = exact match.")]
    public bool useNormalizedSegmentMatching = true;

    [Header("Timing")]
    [Min(0.1f)] public float requiredSeconds = 3f;

    [Header("Hand Filtering")]
    public LayerMask handLayers;

    [Header("Feedback Spawn")]
    public Transform feedbackAnchor;
    public Vector3 feedbackOffset = new Vector3(0f, 0.15f, 0f);

    [Header("Wrong Feedback")]
    public bool showWrongFeedbackWhenNotAtStage = true;
    [Min(0f)] public float wrongFeedbackCooldown = 1.0f;

    [Header("Dependencies")]
    [SerializeField] private WDialogueManager dialogueManager;
    [SerializeField] private WaterTapXR waterTap;

    private Collider triggerCollider;
    private readonly HashSet<Collider> inside = new HashSet<Collider>();

    private float timer = 0f;

    // Session gating
    private bool eligibleForCompletion = false;
    private bool blockedUntilExit = false;

    // Which stage we armed on (prevents “arm on one stage, complete on another”)
    private int armedStageIndex = -1;

    private float lastWrongTime = -999f;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;

        if (dialogueManager == null)
            dialogueManager = WDialogueManager.Instance;

        if (waterTap == null)
        {
            Debug.LogError($"{name}: WaterTapXR reference is missing.");
            enabled = false;
            return;
        }

        // Start disabled; enabled only while faucet is ON
        triggerCollider.enabled = false;

        waterTap.OnTapStateChanged += HandleTapStateChanged;
    }

    private void OnDestroy()
    {
        if (waterTap != null)
            waterTap.OnTapStateChanged -= HandleTapStateChanged;
    }

    private void HandleTapStateChanged(bool isOpen)
    {
        triggerCollider.enabled = isOpen;

        if (!isOpen)
            ResetSession();
    }

    private void Update()
    {
        if (!triggerCollider.enabled) return;
        if (dialogueManager == null) return;
        if (inside.Count == 0) return;

        if (blockedUntilExit)
        {
            timer = 0f;
            return;
        }

        if (!eligibleForCompletion || armedStageIndex < 0 || armedStageIndex >= allowedStages.Count)
        {
            timer = 0f;
            return;
        }

        // Must remain on the exact stage we armed on
        StageGate armed = allowedStages[armedStageIndex];
        if (!IsAtStage(armed.segmentName, armed.lineIndex))
        {
            timer = 0f;
            eligibleForCompletion = false;
            armedStageIndex = -1;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= requiredSeconds)
        {
            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportCorrect(spawnPos);

            // --- SCORE: correct completion of the armed stage ---
            ScoreManager.Instance?.RegisterCorrect(armed.segmentName, armed.lineIndex);

            // Block until exit to avoid rapid retriggers while hands are still inside
            blockedUntilExit = true;
            eligibleForCompletion = false;
            armedStageIndex = -1;
            timer = 0f;

            // Advance trigger-gated dialogue line
            dialogueManager.AdvanceDialogue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidHand(other)) return;

        inside.Add(other);

        if (blockedUntilExit) return;

        int stageIndex = GetMatchingStageIndex();
        if (stageIndex >= 0)
        {
            // Arm completion only when a hand ENTERS while at a valid stage
            eligibleForCompletion = true;
            armedStageIndex = stageIndex;
            timer = 0f;
            return;
        }

        // Wrong attempt: show wrong feedback and block until exit (prevents wrong+correct combos)
        if (showWrongFeedbackWhenNotAtStage && Time.time - lastWrongTime >= wrongFeedbackCooldown)
        {
            lastWrongTime = Time.time;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportWrong(spawnPos);

            // --- SCORE: mistake registered on actual current stage ---
            ScoreManager.Instance?.RegisterMistake(
                dialogueManager.GetCurrentSegmentName(),
                dialogueManager.GetCurrentLineIndex(),
                "Hands entered timed zone at wrong stage"
            );
        }

        blockedUntilExit = true;
        eligibleForCompletion = false;
        armedStageIndex = -1;
        timer = 0f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (inside.Remove(other) && inside.Count == 0)
            ResetSession();
    }

    private int GetMatchingStageIndex()
    {
        if (dialogueManager == null) return -1;
        if (allowedStages == null || allowedStages.Count == 0) return -1;

        for (int i = 0; i < allowedStages.Count; i++)
        {
            StageGate g = allowedStages[i];
            if (IsAtStage(g.segmentName, g.lineIndex))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Manual Inspector-driven stage matching.
    /// If useNormalizedSegmentMatching is ON, it ignores casing/spaces.
    /// </summary>
    private bool IsAtStage(string inspectorSegmentName, int inspectorLineIndex)
    {
        if (dialogueManager == null) return false;

        if (useNormalizedSegmentMatching)
        {
            string currentSegKey = WDialogueManager.NormalizeSegmentKey(dialogueManager.GetCurrentSegmentName());
            string targetSegKey = WDialogueManager.NormalizeSegmentKey(inspectorSegmentName);
            return currentSegKey == targetSegKey && dialogueManager.GetCurrentLineIndex() == inspectorLineIndex;
        }
        else
        {
            // Strict: exact name match
            return dialogueManager.GetCurrentSegmentName() == inspectorSegmentName
                   && dialogueManager.GetCurrentLineIndex() == inspectorLineIndex;
        }
    }

    private void ResetSession()
    {
        inside.Clear();
        timer = 0f;
        eligibleForCompletion = false;
        blockedUntilExit = false;
        armedStageIndex = -1;
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        Transform anchor = feedbackAnchor != null ? feedbackAnchor : transform;
        return anchor.position + feedbackOffset;
    }

    private bool IsValidHand(Collider col)
    {
        int mask = 1 << col.gameObject.layer;
        return (handLayers.value & mask) != 0;
    }
}
