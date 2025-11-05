using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Handles checklist UI updates triggered by the DialogueManager.
/// Keeps Taskboard visuals but uses simple CompleteTask() logic.
/// </summary>
public class DialogueChecklist : MonoBehaviour
{
    [System.Serializable]
    public class TaskItem
    {
        public string taskName;                  // e.g. "Sanitize"
        public TextMeshProUGUI taskLabel;        // UI label for this task
        public Toggle taskToggle;                // UI toggle if your Taskboard uses one
        [HideInInspector] public bool isDone = false;
    }

    [Header("Taskboard Checklist Items")]
    public List<TaskItem> tasks = new List<TaskItem>();

    public static DialogueChecklist Instance;

    void Awake()
    {
        Instance = this;

        // Initialize UI
        foreach (var task in tasks)
        {
            if (task.taskLabel != null)
                task.taskLabel.text = task.taskName;

            if (task.taskToggle != null)
                task.taskToggle.isOn = false;

            task.isDone = false;
        }
    }

    /// <summary>
    /// Called externally (e.g., from DialogueManager) when a task should be marked complete.
    /// </summary>
    public void CompleteTask(string taskName)
    {
        if (string.IsNullOrWhiteSpace(taskName)) return;

        // Normalize name for matching
        string normalized = taskName.ToLower().Trim();
        var task = tasks.Find(t => t.taskName.ToLower().Trim() == normalized);

        if (task == null)
        {
            Debug.LogWarning($"⚠️ DialogueChecklist: Task '{taskName}' not found in UI list.");
            return;
        }

        if (task.isDone)
        {
            Debug.Log($"✔️ Task '{task.taskName}' already completed.");
            return;
        }

        // Mark completed
        task.isDone = true;
        if (task.taskLabel != null)
            task.taskLabel.text = $"☑ {task.taskName}";

        if (task.taskToggle != null)
            task.taskToggle.isOn = true;

        Debug.Log($"✅ DialogueChecklist: Completed task '{task.taskName}'");
    }

    /// <summary>
    /// Reset all tasks (optional — for testing or replaying scene).
    /// </summary>
    public void ResetChecklist()
    {
        foreach (var task in tasks)
        {
            task.isDone = false;
            if (task.taskLabel != null)
                task.taskLabel.text = task.taskName;
            if (task.taskToggle != null)
                task.taskToggle.isOn = false;
        }

        Debug.Log("🔁 DialogueChecklist reset.");
    }
}
