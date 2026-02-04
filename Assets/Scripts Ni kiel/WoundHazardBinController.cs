using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class WoundHazardBinController : MonoBehaviour
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

    [Header("Feedback")]
    [Tooltip("Spawn feedback above bin by this offset.")]
    [SerializeField] private Vector3 feedbackOffset = new Vector3(0f, 0.25f, 0f);

    [Header("Content Behavior")]
    [Tooltip("Destroy objects after the bin closes.")]
    [SerializeField] private bool destroyContentsOnClose = true;

    [Header("Dialogue: Towel Disposal (Optional)")]
    [SerializeField] private bool advanceDialogueOnTowelDrop = true;
    [SerializeField] private string towelTag = "towel";
    [SerializeField] private string towelTargetSegmentName = "Handwashing";
    [SerializeField] private int towelTargetLineIndex = 6;

    [Header("Dialogue: Needle Disposal Steps")]
    [SerializeField] private bool advanceDialogueOnNeedleDrop = true;
    [SerializeField] private string needleTag = "Needle";

    [System.Serializable]
    public class NeedleDisposalStage
    {
        public string segmentName;
        public int lineIndex;
        [HideInInspector] public bool completed;
    }

    [SerializeField]
    private List<NeedleDisposalStage> needleDisposalStages = new List<NeedleDisposalStage>()
    {
        new NeedleDisposalStage { segmentName = "Vaccine Prep", lineIndex = 6 },
        new NeedleDisposalStage { segmentName = "Injection", lineIndex = 3 }
    };

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

        if (contentsTrigger == null)
        {
            Debug.LogWarning(name + ": ContentsTrigger is not assigned.");
        }
        else if (!contentsTrigger.isTrigger)
        {
            Debug.LogWarning(name + ": ContentsTrigger should be IsTrigger.");
        }

        if (proximityTrigger != null && !proximityTrigger.isTrigger)
        {
            Debug.LogWarning(name + ": ProximityTrigger should be IsTrigger.");
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        SetOpen(true);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        SetOpen(false);

        if (destroyContentsOnClose)
            Invoke(nameof(DestroyContents), destroyDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleContentsEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleContentsExit(other);
    }

    private void HandleContentsEnter(Collider other)
    {
        if (other == null) return;

        GameObject obj = other.gameObject;

        if (!contents.Contains(obj))
            contents.Add(obj);

        if (advanceDialogueOnTowelDrop && !towelStepCompleted && obj.CompareTag(towelTag))
        {
            TryCompleteTowelStep();
        }

        if (advanceDialogueOnNeedleDrop && obj.CompareTag(needleTag))
        {
            TryCompleteNeedleStep();
        }
    }

    private void TryCompleteTowelStep()
    {
        WoundDialogueManager dm = WoundDialogueManager.Instance;
        if (dm == null) return;

        if (dm.IsAtStage(towelTargetSegmentName, towelTargetLineIndex))
        {
            towelStepCompleted = true;

            ScoreManager.Instance?.RegisterCorrect(towelTargetSegmentName, towelTargetLineIndex);
            FeedbackManager.Instance?.ReportCorrect(GetFeedbackSpawnPosition());

            dm.AdvanceDialogue();

            Debug.Log(name + ": Towel disposed correctly.");
        }
        else
        {
            FeedbackManager.Instance?.ReportWrong(GetFeedbackSpawnPosition());

            ScoreManager.Instance?.RegisterMistake(
                dm.GetCurrentSegmentName(),
                dm.GetCurrentLineIndex(),
                "Towel dropped in bin at wrong stage"
            );
        }
    }

    private void TryCompleteNeedleStep()
    {
        WoundDialogueManager dm = WoundDialogueManager.Instance;
        if (dm == null) return;

        for (int i = 0; i < needleDisposalStages.Count; i++)
        {
            NeedleDisposalStage stage = needleDisposalStages[i];
            if (stage == null || stage.completed) continue;

            if (dm.IsAtStage(stage.segmentName, stage.lineIndex))
            {
                stage.completed = true;

                ScoreManager.Instance?.RegisterCorrect(stage.segmentName, stage.lineIndex);
                FeedbackManager.Instance?.ReportCorrect(GetFeedbackSpawnPosition());

                dm.AdvanceDialogue();

                Debug.Log(name + ": Needle disposed correctly.");
                return;
            }
        }

        FeedbackManager.Instance?.ReportWrong(GetFeedbackSpawnPosition());

        ScoreManager.Instance?.RegisterMistake(
            dm.GetCurrentSegmentName(),
            dm.GetCurrentLineIndex(),
            "Needle dropped in hazards bin at wrong stage"
        );
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
                Destroy(obj);
        }

        contents.Clear();
    }

    private void SetOpen(bool open)
    {
        if (animator == null) return;
        animator.SetBool(openParam, open);
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        return transform.position + feedbackOffset;
    }

    public void ResetTowelStep()
    {
        towelStepCompleted = false;
    }

    public void ResetNeedleSteps()
    {
        foreach (var stage in needleDisposalStages)
        {
            if (stage != null)
                stage.completed = false;
        }
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
        }
    }
}
