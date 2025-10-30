using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SyringeInteractionTrigger : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;
    public AttachablePart syringeCap;  // assign your Syringe_Cap here
    public string targetSegmentName = "vaccine prep";
    public int targetLineIndex = 3;

    [Header("Settings")]
    public string vialTag = "DiluentVial"; // tag for the vial collider
    public bool canTriggerOnce = true;

    private bool hasTriggered = false;

    private void Start()
    {
        if (dialogueManager == null)
            dialogueManager = FindObjectOfType<DialogueManager>();

        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ensure we're colliding with the vial
        if (!other.CompareTag(vialTag)) return;

        // Prevent multiple triggers
        if (canTriggerOnce && hasTriggered) return;

        // Check if cap is detached
        if (syringeCap != null && syringeCap.isAttached)
        {
            Debug.Log("Syringe cap still attached - cannot interact yet.");
            return;
        }

        // Verify dialogue stage
        string currentSegment = dialogueManager.GetCurrentSegmentName().ToLower().Trim();
        int currentLine = dialogueManager.GetCurrentLineIndex();

        if (currentSegment == targetSegmentName.ToLower().Trim() && currentLine == targetLineIndex)
        {
            dialogueManager.AdvanceDialogue();
            hasTriggered = true;
            Debug.Log($"Syringe triggered diluent interaction ({targetSegmentName}:{targetLineIndex})");
        }
        else
        {
            Debug.Log($"Syringe collision ignored ({currentSegment}:{currentLine})");
        }
    }
}
