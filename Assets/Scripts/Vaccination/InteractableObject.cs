using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string objectName = "Interactable";
    public bool advanceDialogueOnUse = true;

    [Header("Dialogue Trigger Target")]
    public string targetSegmentName;
    public int targetLineIndex;

    [Header("Feedback")]
    public bool showWrongFeedbackWhenNotAtStage = true;
    [Min(0f)] public float wrongFeedbackCooldown = 1.0f;

    [Tooltip("If assigned, feedback spawns at this transform. Otherwise uses this object's transform.")]
    public Transform feedbackAnchor;

    [Tooltip("Offset added to feedback spawn position.")]
    public Vector3 feedbackOffset = new Vector3(0f, 0.15f, 0f);

    [Header("References (Optional)")]
    [SerializeField] private DialogueManager dialogueManager;

    private XRBaseInteractable interactable;
    private float lastWrongTime = -999f;

    private void Awake()
    {
        if (string.IsNullOrEmpty(objectName))
            objectName = gameObject.name;

        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;

        interactable = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(OnSelect);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelect);
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        if (dialogueManager == null) return;

        bool atStage = dialogueManager.IsAtStage(targetSegmentName, targetLineIndex);

        if (atStage)
        {
            if (advanceDialogueOnUse)
            {
                Vector3 spawnPos = GetFeedbackSpawnPosition();
                FeedbackManager.Instance?.ReportCorrect(spawnPos);

                dialogueManager.AdvanceDialogue();
                Debug.Log($"{objectName} triggered dialogue (Correct) at {targetSegmentName}:{targetLineIndex}");
            }
            return;
        }

        if (showWrongFeedbackWhenNotAtStage && Time.time - lastWrongTime >= wrongFeedbackCooldown)
        {
            lastWrongTime = Time.time;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportWrong(spawnPos);

            Debug.Log($"{objectName} used at wrong stage (current {DialogueManager.NormalizeSegmentKey(dialogueManager.GetCurrentSegmentName())}:{dialogueManager.GetCurrentLineIndex()})");
        }
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        Transform anchor = feedbackAnchor != null ? feedbackAnchor : transform;
        return anchor.position + feedbackOffset;
    }
}
