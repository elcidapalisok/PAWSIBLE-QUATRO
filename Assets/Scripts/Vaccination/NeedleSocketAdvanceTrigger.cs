using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class NeedleSocketAdvanceTrigger : MonoBehaviour
{
    [Header("Dialogue Gate")]
    public string targetSegmentName = "Vaccine Prep";
    public int targetLineIndex = 7;

    [Header("Feedback Spawn")]
    public Transform feedbackAnchor;
    public Vector3 feedbackOffset = new Vector3(0f, 0.15f, 0f);

    [Header("Wrong Feedback (only when armed)")]
    public bool showWrongFeedbackWhenNotAtStage = true;
    [Min(0f)] public float wrongFeedbackCooldown = 1.0f;

    [Header("Arming")]
    [Tooltip("If true, the socket will only start counting after we reach the target stage once.")]
    public bool armOnlyWhenStageReached = true;

    private XRSocketInteractor socket;

    private bool armed = false;
    private bool completed = false;

    private float lastWrongTime = -999f;

    private void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnSocketSelectEntered);
    }

    private void OnDestroy()
    {
        if (socket != null)
            socket.selectEntered.RemoveListener(OnSocketSelectEntered);
    }

    private void Update()
    {
        if (completed) return;
        if (!armOnlyWhenStageReached) return;

        var dm = DialogueManager.Instance;
        if (dm == null) return;

        // Arm the trigger ONLY when we reach the required stage
        if (!armed && dm.IsAtStage(targetSegmentName, targetLineIndex))
        {
            armed = true;
            // We don't count the currently attached needle as an action.
            Debug.Log($"{name}: Needle socket trigger ARMED at ({targetSegmentName}:{targetLineIndex}).");
        }
    }

    private void OnSocketSelectEntered(SelectEnterEventArgs args)
    {
        if (completed) return;

        // If we require arming, ignore any socket events before arming (this fixes startup auto-select)
        if (armOnlyWhenStageReached && !armed)
            return;

        DialogueManager dm = DialogueManager.Instance;
        if (dm == null) return;

        // Success only if we are at the correct stage
        if (dm.IsAtStage(targetSegmentName, targetLineIndex))
        {
            completed = true;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportCorrect(spawnPos);
            ScoreManager.Instance?.RegisterCorrect(targetSegmentName, targetLineIndex);

            dm.AdvanceDialogue();
            return;
        }

        // Wrong feedback only when armed
        if (showWrongFeedbackWhenNotAtStage && Time.time - lastWrongTime >= wrongFeedbackCooldown)
        {
            lastWrongTime = Time.time;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportWrong(spawnPos);

            ScoreManager.Instance?.RegisterMistake(
                dm.GetCurrentSegmentName(),
                dm.GetCurrentLineIndex(),
                "Needle attached to syringe at wrong stage"
            );
        }
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        Transform anchor = feedbackAnchor != null ? feedbackAnchor : transform;
        return anchor.position + feedbackOffset;
    }
}
