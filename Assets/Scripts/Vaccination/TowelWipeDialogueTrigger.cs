using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(BoxCollider))]
public class TowelWipeDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Stage Gate")]
    public string targetSegmentName = "Handwashing";
    public int targetLineIndex = 5;

    [Header("Timing")]
    [Min(0.1f)] public float requiredSeconds = 3f;

    [Header("Hand Filtering")]
    public LayerMask handLayers;

    [Header("Grab Requirement")]
    public bool requireGrabbed = true;
    [SerializeField] private XRGrabInteractable grabInteractable;

    [Header("Feedback Spawn")]
    public Transform feedbackAnchor;
    public Vector3 feedbackOffset = new Vector3(0f, 0.15f, 0f);

    [Header("Wrong Feedback")]
    public bool showWrongFeedbackWhenNotAtStage = true;
    [Min(0f)] public float wrongFeedbackCooldown = 1.0f;

    [Header("Overlap Check")]
    [Tooltip("Extra padding (meters) added to the wipe bounds to make wiping easier.")]
    [Min(0f)] public float overlapPadding = 0.02f;

    [Header("Debug")]
    public bool debugLogs = true;

    private DialogueManager dialogueManager;
    private BoxCollider box;

    private float timer = 0f;
    private bool completed = false;
    private float lastWrongTime = -999f;
    private float lastLogTime = -999f;

    private readonly Collider[] results = new Collider[32];

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
        box.isTrigger = true;

        dialogueManager = DialogueManager.Instance;

        if (grabInteractable == null)
            grabInteractable = GetComponentInParent<XRGrabInteractable>();
    }

    private void Update()
    {
        if (completed) return;
        if (dialogueManager == null) return;

        bool grabbed = (grabInteractable != null && grabInteractable.isSelected);
        bool atStage = dialogueManager.IsAtStage(targetSegmentName, targetLineIndex);

        if (debugLogs && Time.time - lastLogTime > 0.5f)
        {
            lastLogTime = Time.time;
            Debug.Log($"[TowelWipe] grabbed={grabbed}, requireGrabbed={requireGrabbed}, atStage={atStage}, timer={timer:F2}");
        }

        if (requireGrabbed && !grabbed)
        {
            timer = 0f;
            return;
        }

        bool overlapping = IsAnyHandOverlapping();
        if (!overlapping)
        {
            timer = 0f;
            return;
        }

        if (!atStage)
        {
            timer = 0f;

            if (showWrongFeedbackWhenNotAtStage && Time.time - lastWrongTime >= wrongFeedbackCooldown)
            {
                lastWrongTime = Time.time;
                FeedbackManager.Instance?.ReportWrong(GetFeedbackSpawnPosition());
            }

            return;
        }

        timer += Time.deltaTime;

        if (timer >= requiredSeconds)
        {
            completed = true;
            FeedbackManager.Instance?.ReportCorrect(GetFeedbackSpawnPosition());
            dialogueManager.AdvanceDialogue();
        }
    }

    private bool IsAnyHandOverlapping()
    {
        // Use WORLD bounds to avoid any local scale confusion
        Bounds b = box.bounds;
        Vector3 center = b.center;
        Vector3 half = b.extents + Vector3.one * overlapPadding;

        int count = Physics.OverlapBoxNonAlloc(
            center,
            half,
            results,
            Quaternion.identity,           // bounds are axis-aligned
            handLayers,
            QueryTriggerInteraction.Collide // detects hands even if they were triggers (safe)
        );

        if (debugLogs && count > 0)
        {
            Debug.Log($"[TowelWipe] OverlapBox count={count}, first={results[0]?.name}");
        }

        return count > 0;
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        Transform anchor = feedbackAnchor != null ? feedbackAnchor : transform;
        return anchor.position + feedbackOffset;
    }
}
