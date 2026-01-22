using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class HazardBinController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private string openParam = "isOpen";
    [SerializeField] private float destroyDelay = 0.2f;

    [Header("Contents Trigger (Small - inside bin)")]
    [Tooltip("Assign the small trigger collider inside the bin where objects are dropped.")]
    [SerializeField] private Collider contentsTrigger;

    [Header("Proximity Trigger (Large - optional)")]
    [Tooltip("Assign a larger trigger collider around the bin to auto-open when the user enters.")]
    [SerializeField] private Collider proximityTrigger;

    [Header("Dialogue Advancement")]
    [Tooltip("Tag used to identify towel objects.")]
    [SerializeField] private string towelTag = "towel";

    [Tooltip("If true, placing a towel advances dialogue at Handwashing:6.")]
    [SerializeField] private bool advanceDialogueOnTowelDrop = true;

    [SerializeField] private string targetSegmentName = "Handwashing";
    [SerializeField] private int targetLineIndex = 6;

    private Animator animator;
    private XRSimpleInteractable interactable;

    private readonly List<GameObject> contents = new List<GameObject>();
    private bool towelStepCompleted = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        interactable = GetComponent<XRSimpleInteractable>();

        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);

        // Safety checks
        if (contentsTrigger == null)
        {
            Debug.LogWarning($"{name}: ContentsTrigger is not assigned. Bin will not detect dropped objects.");
        }
        else if (!contentsTrigger.isTrigger)
        {
            Debug.LogWarning($"{name}: ContentsTrigger should be set as IsTrigger.");
        }

        if (proximityTrigger != null && !proximityTrigger.isTrigger)
        {
            Debug.LogWarning($"{name}: ProximityTrigger should be set as IsTrigger.");
        }
    }

    // -------------------------
    // XR HOVER EVENTS (existing behavior)
    // -------------------------
    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        SetOpen(true);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        SetOpen(false);
        Invoke(nameof(DestroyContents), destroyDelay);
    }

    // -------------------------
    // TRIGGER ROUTING
    // -------------------------
    private void OnTriggerEnter(Collider other)
    {
        HandleContentsEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleContentsExit(other);
    }

    // -------------------------
    // CONTENT HANDLING
    // -------------------------
    private void HandleContentsEnter(Collider other)
    {
        if (other == null) return;

        if (!contents.Contains(other.gameObject))
        {
            contents.Add(other.gameObject);
        }

        // Dialogue advancement: towel dropped
        if (advanceDialogueOnTowelDrop && !towelStepCompleted && other.CompareTag(towelTag))
        {
            DialogueManager dm = DialogueManager.Instance;
            if (dm != null && dm.IsAtStage(targetSegmentName, targetLineIndex))
            {
                towelStepCompleted = true;

                // --- SCORE: correct completion of this step ---
                ScoreManager.Instance?.RegisterCorrect(targetSegmentName, targetLineIndex);

                FeedbackManager.Instance?.ReportCorrect(GetFeedbackSpawnPosition());
                dm.AdvanceDialogue();

                Debug.Log($"{name}: Towel detected in bin. Advanced dialogue ({targetSegmentName}:{targetLineIndex}).");
            }
            else
            {
                // Wrong feedback + scoring
                FeedbackManager.Instance?.ReportWrong(GetFeedbackSpawnPosition());

                if (dm != null)
                {
                    ScoreManager.Instance?.RegisterMistake(
                        dm.GetCurrentSegmentName(),
                        dm.GetCurrentLineIndex(),
                        "Towel dropped in bin at wrong stage"
                    );
                }

                Debug.Log($"{name}: Towel detected but not at correct stage. No advance.");
            }
        }
    }

    private void HandleContentsExit(Collider other)
    {
        if (other == null) return;
        contents.Remove(other.gameObject);
    }

    private void DestroyContents()
    {
        foreach (GameObject obj in contents)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        contents.Clear();
    }

    // -------------------------
    // OPEN/CLOSE HELPERS
    // -------------------------
    private void SetOpen(bool open)
    {
        if (animator == null) return;
        animator.SetBool(openParam, open);
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        // Spawn feedback above bin
        return transform.position + Vector3.up * 0.25f;
    }

    // -------------------------
    // PUBLIC API (optional)
    // -------------------------
    public void ResetTowelStep()
    {
        towelStepCompleted = false;
    }

    // -------------------------
    // SAFETY
    // -------------------------
    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
        }
    }
}
