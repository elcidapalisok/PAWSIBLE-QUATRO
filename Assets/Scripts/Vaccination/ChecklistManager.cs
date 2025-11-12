using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ChecklistManager : MonoBehaviour
{
    [System.Serializable]
    public class TaskItem
    {
        public string taskName;
        public TextMeshProUGUI taskText;
        [HideInInspector] public bool isCompleted = false;
    }

    [Header("Checklist Items")]
    public List<TaskItem> tasks = new List<TaskItem>();

    public static ChecklistManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void CompleteTask(string taskName)
    {
        TaskItem task = tasks.Find(t => t.taskName.ToLower().Trim() == taskName.ToLower().Trim());
        if (task != null && !task.isCompleted)
        {
            task.isCompleted = true;
            if (task.taskText != null)
                task.taskText.text = $"☑ {task.taskName}";

            Debug.Log($"✅ Task completed: {task.taskName}");
        }
        else
        {
            Debug.LogWarning($"⚠ Task not found or already completed: {taskName}");
        }
    }

    // <-- NEW METHOD -->
    public bool IsTaskCompleted(string taskName)
    {
        TaskItem task = tasks.Find(t => t.taskName.ToLower().Trim() == taskName.ToLower().Trim());
        return task != null && task.isCompleted;
    }
}
