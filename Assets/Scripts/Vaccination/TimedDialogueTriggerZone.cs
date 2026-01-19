using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TimedDialogueTriggerZone : MonoBehaviour
{
    [Header("Dialogue Stage Gate")]
    public string targetSegmentName;
    public int targetLineIndex;

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

    // Arms timing only when a hand ENTERS while stage is correct.
    private bool eligibleForCompletion = false;

    // NEW: once wrong happens, you must fully EXIT before correct can ever happen.
    private bool blockedUntilExit = false;

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
        {
            ResetSession();
        }
    }

    private void Update()
    {
        if (!triggerCollider.enabled) return;
        if (dialogueManager == null) return;

        // If nothing inside, do nothing.
        if (inside.Count == 0) return;

        // If blocked, do not allow completion.
        if (blockedUntilExit)
        {
            timer = 0f;
            return;
        }

        // Must be eligible (entered while correct stage).
        if (!eligibleForCompletion)
        {
            timer = 0f;
            return;
        }

        // Must still be at correct stage; otherwise force re-entry.
        if (!dialogueManager.IsAtStage(targetSegmentName, targetLineIndex))
        {
            timer = 0f;
            eligibleForCompletion = false;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= requiredSeconds)
        {
            // SUCCESS: show correct feedback and advance.
            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportCorrect(spawnPos);

            // Block until exit so you can't get another correct immediately while still inside.
            blockedUntilExit = true;
            eligibleForCompletion = false;
            timer = 0f;

            dialogueManager.AdvanceDialogue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidHand(other)) return;

        inside.Add(other);

        // If already blocked, do nothing until user fully exits.
        if (blockedUntilExit) return;

        bool atStage = dialogueManager != null && dialogueManager.IsAtStage(targetSegmentName, targetLineIndex);

        if (atStage)
        {
            // Arm timer only when a hand ENTERS at correct stage.
            eligibleForCompletion = true;
            timer = 0f; // restart cleanly on entry
            return;
        }

        // WRONG attempt: show wrong feedback and block until exit.
        if (showWrongFeedbackWhenNotAtStage && Time.time - lastWrongTime >= wrongFeedbackCooldown)
        {
            lastWrongTime = Time.time;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportWrong(spawnPos);
        }

        blockedUntilExit = true;
        eligibleForCompletion = false;
        timer = 0f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (inside.Remove(other))
        {
            if (inside.Count == 0)
            {
                // Only when ALL hands exit do we allow a new session.
                ResetSession();
            }
        }
    }

    private void ResetSession()
    {
        inside.Clear();
        timer = 0f;
        eligibleForCompletion = false;
        blockedUntilExit = false;
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
