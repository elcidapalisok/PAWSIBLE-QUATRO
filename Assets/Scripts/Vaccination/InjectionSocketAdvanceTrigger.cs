using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Collider))]
public class InjectionAdvanceTrigger : MonoBehaviour
{
    [Header("Dialogue Gate")]
    public string targetSegmentName = "Injection";
    public int targetLineIndex = 9;

    [Header("Needle Detection")]
    [Tooltip("Tag used on the needle tip collider.")]
    public string needleTipTag = "NeedleTip";

    [Header("Optional Syringe Animation")]
    public bool playSyringeAnimation = true;

    [Tooltip("Injection should usually be Empty (push).")]
    public SyringeAnimMode animationMode = SyringeAnimMode.Empty;

    public enum SyringeAnimMode
    {
        Fill,
        Empty
    }

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

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;

        var col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && fired) return;
        if (dialogueManager == null) return;
        if (other == null) return;

        if (!other.CompareTag(needleTipTag))
            return;

        // Correct stage
        if (dialogueManager.IsAtStage(targetSegmentName, targetLineIndex))
        {
            fired = true;

            if (playSyringeAnimation)
                TryAnimateFromNeedleTipViaSocket(other);

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportCorrect(spawnPos);
            ScoreManager.Instance?.RegisterCorrect(targetSegmentName, targetLineIndex);

            dialogueManager.AdvanceDialogue();
            return;
        }

        // Wrong stage feedback
        if (showWrongFeedbackWhenNotAtStage && Time.time - lastWrongTime >= wrongFeedbackCooldown)
        {
            lastWrongTime = Time.time;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportWrong(spawnPos);

            ScoreManager.Instance?.RegisterMistake(
                dialogueManager.GetCurrentSegmentName(),
                dialogueManager.GetCurrentLineIndex(),
                "Needle entered dog injection trigger at wrong stage"
            );
        }
    }

    private void TryAnimateFromNeedleTipViaSocket(Collider needleTip)
    {
        if (needleTip == null) return;

        // Find the needle XRGrabInteractable
        XRGrabInteractable needleGrab = needleTip.GetComponentInParent<XRGrabInteractable>();
        if (needleGrab == null)
            return;

        // If needle is attached, selecting interactor is a socket
        XRSocketInteractor socket = needleGrab.firstInteractorSelecting as XRSocketInteractor;
        if (socket == null)
        {
            // Needle touched injection trigger but is not currently socketed into a syringe
            return;
        }

        // Socket should live under syringe_body hierarchy
        SyringePlungerAnimator plunger = socket.GetComponentInParent<SyringePlungerAnimator>();
        if (plunger != null)
        {
            if (animationMode == SyringeAnimMode.Fill) plunger.PlayFill();
            else plunger.PlayEmpty();
            return;
        }

        // Fallback: try Animator
        Animator anim = socket.GetComponentInParent<Animator>();
        if (anim != null)
        {
            if (animationMode == SyringeAnimMode.Fill) anim.Play("fill", 0, 0f);
            else anim.Play("empty", 0, 0f);
        }
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        Transform anchor = feedbackAnchor != null ? feedbackAnchor : transform;
        return anchor.position + feedbackOffset;
    }
}
