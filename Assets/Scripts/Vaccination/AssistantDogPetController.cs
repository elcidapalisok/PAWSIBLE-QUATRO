using UnityEngine;

public class AssistantDogPetController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator assistantAnimator;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Animator Parameter Names")]
    [SerializeField] private string isPettingParam = "IsPetting";

    [Header("Stop Petting At This Dialogue Stage")]
    [SerializeField] private string stopSegmentName = "Injection";
    [SerializeField] private int stopLineIndex = 9;

    [Header("Optional: Start Petting At This Dialogue Stage")]
    [Tooltip("Enable this if you want the script to also turn petting ON at a specific stage.")]
    [SerializeField] private bool controlStartPetting = false;
    [SerializeField] private string startSegmentName = "Injection";
    [SerializeField] private int startLineIndex = 0;

    [Header("Optional: Force Hold State Immediately")]
    [Tooltip("If set, Play() this state when stopping to ensure instant hold pose.")]
    [SerializeField] private bool forceHoldStateOnStop = false;
    [SerializeField] private string holdStateName = "Hold_dog";

    private bool lastIsPetting;

    private void Awake()
    {
        if (assistantAnimator == null)
            assistantAnimator = GetComponentInChildren<Animator>();

        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;
    }

    private void Start()
    {
        // Initialize based on current dialogue stage.
        ApplyPettingState(force: true);
    }

    private void Update()
    {
        ApplyPettingState(force: false);
    }

    private void ApplyPettingState(bool force)
    {
        if (assistantAnimator == null) return;
        if (dialogueManager == null) return;

        bool shouldPet = lastIsPetting;

        // Optional: turn petting ON at a specific stage.
        if (controlStartPetting &&
            dialogueManager.IsAtStage(startSegmentName, startLineIndex))
        {
            shouldPet = true;
        }

        // Stop petting at the target stage (Injection line 9).
        if (dialogueManager.IsAtStage(stopSegmentName, stopLineIndex))
        {
            shouldPet = false;

            if (forceHoldStateOnStop && !string.IsNullOrEmpty(holdStateName))
            {
                // Forces immediate snap to Hold state (no need to wait for transition).
                assistantAnimator.Play(holdStateName, 0, 0f);
            }
        }

        if (!force && shouldPet == lastIsPetting)
            return;

        lastIsPetting = shouldPet;
        assistantAnimator.SetBool(isPettingParam, shouldPet);
    }
}
