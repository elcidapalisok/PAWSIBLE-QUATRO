using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueReportTracker : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;

    [Header("Summary UI (Optional)")]
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI techniqueText;
    public TextMeshProUGUI procedureText;
    public TextMeshProUGUI efficiencyText;
    public TextMeshProUGUI totalScoreText;

    [Serializable]
    public class RequiredTrigger
    {
        public string triggerId;          // e.g. "EnterWorkstation"
        public string segmentName;        // e.g. "handwashing"
        public int lineIndex;             // e.g. 2
        [TextArea] public string description; // for summary
    }

    [Header("Trigger Sequence (in order)")]
    public List<RequiredTrigger> requiredTriggers = new List<RequiredTrigger>();

    private int expectedTriggerIndex = 0;
    private string currentlyWaitingForTriggerId = null;

    private int correctCount = 0;
    private int wrongCount = 0;
    private int missedCount = 0;

    /// Call this when DialogueManager pauses on a line that requires a trigger
    public void SetWaitingForTrigger(string triggerId)
    {
        currentlyWaitingForTriggerId = triggerId;
        Debug.Log($"[ReportTracker] Waiting for trigger: {triggerId}");
    }

    /// Call this from a trigger zone / interactable when the user completes something
    public void MarkTriggerCompleted(string triggerId)
    {
        Debug.Log($"[ReportTracker] Trigger completed: {triggerId}");

        // If DialogueManager isn't waiting, we can still score it as "wrong timing"
        if (string.IsNullOrEmpty(currentlyWaitingForTriggerId))
        {
            wrongCount++;
            return;
        }

        // If wrong trigger while waiting
        if (triggerId != currentlyWaitingForTriggerId)
        {
            wrongCount++;
            return;
        }

        // Check if it matches the expected sequence trigger
        if (expectedTriggerIndex < requiredTriggers.Count &&
            requiredTriggers[expectedTriggerIndex].triggerId == triggerId)
        {
            correctCount++;
            expectedTriggerIndex++;
        }
        else
        {
            // correct for the current waiting line but out of expected order
            wrongCount++;
        }

        // clear waiting state
        currentlyWaitingForTriggerId = null;

        // advance the dialogue now that requirement is met
        if (dialogueManager != null)
            dialogueManager.AdvanceDialogue();
    }

    /// Call this when dialogue finishes (or when you open the Results canvas)
    public void GenerateReport()
    {
        // if user never did remaining triggers, count as missed
        missedCount = Mathf.Max(0, requiredTriggers.Count - correctCount);

        // simple scoring
        int totalRequired = Mathf.Max(1, requiredTriggers.Count);
        float accuracy = (float)correctCount / totalRequired;

        // You can change these formulas however you like:
        int accuracyPct = Mathf.RoundToInt(accuracy * 100f);
        int techniquePct = Mathf.Clamp(accuracyPct - wrongCount * 5, 0, 100);
        int procedurePct = Mathf.Clamp(accuracyPct, 0, 100);
        int efficiencyPct = Mathf.Clamp(accuracyPct - missedCount * 10, 0, 100);

        int totalScore = Mathf.RoundToInt((accuracyPct + techniquePct + procedurePct + efficiencyPct) / 4f);

        if (accuracyText) accuracyText.text = $"Accuracy %: {accuracyPct}%";
        if (techniqueText) techniqueText.text = $"Technique: {techniquePct}%";
        if (procedureText) procedureText.text = $"Procedure: {procedurePct}%";
        if (efficiencyText) efficiencyText.text = $"Efficiency: {efficiencyPct}%";
        if (totalScoreText) totalScoreText.text = $"Total Score: {totalScore}%";

        Debug.Log($"[ReportTracker] Report generated. Correct={correctCount}, Wrong={wrongCount}, Missed={missedCount}");
    }
}
