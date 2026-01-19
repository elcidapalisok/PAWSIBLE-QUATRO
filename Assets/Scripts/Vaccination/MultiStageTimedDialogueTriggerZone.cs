using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MultiStageTimedDialogueTriggerZone : MonoBehaviour
{
    [System.Serializable]
    public class StageGate
    {
        public string segmentName;
        public int lineIndex;
    }

    [Header("Allowed Dialogue Stages (Any of these can complete the timer)")]
    public List<StageGate> allowedStages = new List<StageGate>();

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
    [SerializeField] private DialogueManager dialogueManager;
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
            dialogueManager = DialogueManager.Instance;

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
        if (!dialogueManager.IsAtStage(armed.segmentName, armed.lineIndex))
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

            // Block until exit to avoid rapid retriggers while hands are still inside
            blockedUntilExit = true;
            eligibleForCompletion = false;
            armedStageIndex = -1;
            timer = 0f;

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
            if (dialogueManager.IsAtStage(g.segmentName, g.lineIndex))
                return i;
        }

        return -1;
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
