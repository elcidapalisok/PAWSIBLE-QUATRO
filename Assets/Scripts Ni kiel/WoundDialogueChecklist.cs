using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Simple checklist UI for the wound module.
/// Call: WoundDialogueChecklist.Instance.CompleteTask("Task Name");
/// </summary>
public class WoundDialogueChecklist : MonoBehaviour
{
    [System.Serializable]
    public class TaskItem
    {
        public string taskName;            // Must match the string you pass to CompleteTask()
        public TextMeshProUGUI taskLabel;  // Label text on the canvas
        public Toggle taskToggle;          // Optional checkbox toggle
        [HideInInspector] public bool isDone = false;
    }

    [Header("Taskboard Checklist Items")]
    public List<TaskItem> tasks = new List<TaskItem>();

    public static WoundDialogueChecklist Instance;

    private void Awake()
    {
        // Safe singleton (prevents Instance being overwritten by duplicates)
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("⚠️ Multiple WoundDialogueChecklist instances found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Initialize UI
        for (int i = 0; i < tasks.Count; i++)
        {
            var task = tasks[i];
            if (task == null) continue;

            task.isDone = false;

            if (task.taskLabel != null)
                task.taskLabel.text = task.taskName;

            if (task.taskToggle != null)
                task.taskToggle.isOn = false;
        }

        Debug.Log($"📋 WoundDialogueChecklist ready. Tasks count: {tasks.Count}");
    }

    // Normalize strings so matching is stable (case/spacing/symbols won't break it)
    private static string Normalize(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        s = s.ToLowerInvariant().Trim();

        // Remove common formatting differences
        s = s.Replace("☑", "");
        s = s.Replace("✅", "");
        s = s.Replace("\n", "");
        s = s.Replace("\r", "");
        s = s.Replace("\t", "");
        s = s.Replace("_", "");
        s = s.Replace("-", "");
        s = s.Replace(" ", "");

        return s;
    }

    /// <summary>
    /// Mark a task complete by name.
    /// The match is case-insensitive and ignores spaces/underscores/dashes.
    /// </summary>
    public void CompleteTask(string taskName)
    {
        if (string.IsNullOrWhiteSpace(taskName)) return;

        string target = Normalize(taskName);

        TaskItem found = null;
        for (int i = 0; i < tasks.Count; i++)
        {
            var t = tasks[i];
            if (t == null) continue;

            if (Normalize(t.taskName) == target)
            {
                found = t;
                break;
            }
        }

        if (found == null)
        {
            Debug.LogWarning($"⚠️ WoundDialogueChecklist: Task '{taskName}' not found. " +
                             $"Make sure your Inspector taskName matches what you call.");
            Debug.Log("📋 Available taskNames:");
            foreach (var t in tasks)
                if (t != null) Debug.Log(" - " + t.taskName);
            return;
        }

        if (found.isDone)
        {
            Debug.Log($"✔️ Task '{found.taskName}' already completed.");
            return;
        }

        found.isDone = true;

        if (found.taskLabel != null)
            found.taskLabel.text = $"☑ {found.taskName}";

        if (found.taskToggle != null)
            found.taskToggle.isOn = true;

        Debug.Log($"✅ WoundDialogueChecklist: Completed task '{found.taskName}'");
    }

    /// <summary>
    /// Reset all tasks (optional for replay/testing).
    /// </summary>
    public void ResetChecklist()
    {
        foreach (var task in tasks)
        {
            if (task == null) continue;

            task.isDone = false;

            if (task.taskLabel != null)
                task.taskLabel.text = task.taskName;

            if (task.taskToggle != null)
                task.taskToggle.isOn = false;
        }

        Debug.Log("🔁 WoundDialogueChecklist reset.");
    }

    /// <summary>
    /// Optional helper for debugging.
    /// </summary>
    public bool IsTaskDone(string taskName)
    {
        if (string.IsNullOrWhiteSpace(taskName)) return false;
        string target = Normalize(taskName);

        foreach (var t in tasks)
        {
            if (t == null) continue;
            if (Normalize(t.taskName) == target) return t.isDone;
        }
        return false;
    }
}
