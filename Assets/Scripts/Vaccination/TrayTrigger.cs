using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrayTrigger : MonoBehaviour
{
    [Header("Required Items")]
    public string dapVialTag = "DAPVial";
    public string diluentTag = "DiluentVial";

    [Header("Dialogue Trigger Settings")]
    public string targetSegmentName = "Vaccine Prep";
    public int targetLineIndex = 2;

    private bool dapVialPresent = false;
    private bool diluentPresent = false;

    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(dapVialTag))
        {
            dapVialPresent = true;
            Debug.Log("DAP Vial placed on tray.");
        }

        if (other.CompareTag(diluentTag))
        {
            diluentPresent = true;
            Debug.Log("Diluent Vial placed on tray.");
        }

        CheckForCompletion();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(dapVialTag))
        {
            dapVialPresent = false;
            Debug.Log("DAP Vial removed from tray.");
        }

        if (other.CompareTag(diluentTag))
        {
            diluentPresent = false;
            Debug.Log("Diluent Vial removed from tray.");
        }
    }

    private void CheckForCompletion()
    {
        if (!dapVialPresent || !diluentPresent)
            return;

        if (dialogueManager == null)
            return;

        string currentSegment = dialogueManager.GetCurrentSegmentName().ToLower().Trim();
        int currentLine = dialogueManager.GetCurrentLineIndex();
        string targetSegment = targetSegmentName.ToLower().Trim();

        // Check if we’re at the correct dialogue step
        if (currentSegment == targetSegment && currentLine == targetLineIndex)
        {
            Debug.Log("Both vials placed on tray — advancing dialogue.");
            dialogueManager.AdvanceDialogue();
        }
        else
        {
            Debug.Log($"Tray condition met, but dialogue is at ({currentSegment}:{currentLine}) instead of ({targetSegment}:{targetLineIndex})");
        }
    }
}
