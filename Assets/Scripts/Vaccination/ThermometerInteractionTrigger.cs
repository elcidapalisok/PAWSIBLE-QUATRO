using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ThermometerInteractionTrigger : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;

    [Header("Target Dialogue Line")]
    public string targetSegmentName = "injection";
    public int targetLineIndex = 1;

    [Header("Settings")]
    public string triggerZoneTag = "DogAnalZone";
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
        if (!other.CompareTag(triggerZoneTag)) return;
        if (canTriggerOnce && hasTriggered) return;

        string currentSegment = dialogueManager.GetCurrentSegmentName().ToLower().Trim();
        int currentLine = dialogueManager.GetCurrentLineIndex();

        if (currentSegment == targetSegmentName.ToLower().Trim() && currentLine == targetLineIndex)
        {
            dialogueManager.AdvanceDialogue();
            hasTriggered = true;
            Debug.Log($"Thermometer interaction triggered: {targetSegmentName}:{targetLineIndex}");
        }
        else
        {
            Debug.Log($"Thermometer trigger ignored: currently at {currentSegment}:{currentLine}");
        }
    }
}
