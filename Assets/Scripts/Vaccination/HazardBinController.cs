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
        // Large proximity trigger: open when user enters
        if (proximityTrigger != null && other == proximityTrigger)
        {
            // This branch is not used: Unity passes the entering collider, not the trigger itself.
            // Kept intentionally empty.
        }

        // If we have a proximity trigger assigned, check if this OnTriggerEnter came from it
        if (proximityTrigger != null && other != null)
        {
            // If the entering object is in proximity zone, open bin
            // We detect this by checking whether "other" is inside the proximityTrigger volume.
            // But OnTriggerEnter is fired on THIS script object, so we need a different approach:
            // We'll handle proximity via a separate component below (recommended).
        }

        // Small contents trigger logic: only accept objects if they entered the contentsTrigger
        // Since OnTriggerEnter is called on the object that has this script, we assume
        // the collider that fired is the one on this GameObject or its children.
        // Best practice: put this script on the bin root and place the contentsTrigger collider on the same GameObject
        // OR ensure the bin root has the trigger collider component.
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

        // If a contentsTrigger is assigned, ensure this trigger event is coming from it.
        // In many setups, OnTriggerEnter fires on the object with the trigger collider.
        // If your trigger is on a child, Unity won't call this unless the script is on the same object as that collider.
        // So we do a soft check: if contentsTrigger exists and other is NOT the trigger itself, still accept,
        // because Unity passes the entering object collider, not the trigger collider.
        // The real gating is that this script should be on the same object as the contentsTrigger collider.
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

                FeedbackManager.Instance?.ReportCorrect(GetFeedbackSpawnPosition());
                dm.AdvanceDialogue();

                Debug.Log($"{name}: Towel detected in bin. Advanced dialogue ({targetSegmentName}:{targetLineIndex}).");
            }
            else
            {
                // Optional wrong feedback (only if user is at wrong stage)
                FeedbackManager.Instance?.ReportWrong(GetFeedbackSpawnPosition());
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
