using UnityEngine;

public class DogInjectionTrigger : MonoBehaviour
{
    [Header("Dialogue Target")]
    public string targetSegmentName = "Injection";
    public int targetLineIndex = 3; // From Element 3 to Element 4

    private DialogueManager dialogueManager;
    private bool hasTriggered = false;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null)
            Debug.LogError("DialogueManager not found in the scene!");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if this collider belongs to the syringe tip or one of its children
        if (IsSyringe(other.gameObject))
        {
            if (hasTriggered) return; // prevent double triggers
            hasTriggered = true;

            string currentSegment = dialogueManager.GetCurrentSegmentName().ToLower().Trim();
            int currentLine = dialogueManager.GetCurrentLineIndex();

            // Check if this trigger is active for the current dialogue state
            if (currentSegment == targetSegmentName.ToLower().Trim() && currentLine == targetLineIndex)
            {
                dialogueManager.AdvanceDialogue();
                Debug.Log("Syringe correctly injected - advancing dialogue!");
            }
            else
            {
                Debug.Log("Syringe touched dog, but dialogue is not at the correct line.");
            }
        }
    }

    // Helper function that checks if the collider belongs to the syringe or one of its children
    private bool IsSyringe(GameObject obj)
    {
        if (obj.CompareTag("Syringe"))
            return true;

        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            if (parent.CompareTag("Syringe"))
                return true;
            parent = parent.parent;
        }
        return false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsSyringe(other.gameObject))
            hasTriggered = false; // allow re-triggering if needed
    }
}
