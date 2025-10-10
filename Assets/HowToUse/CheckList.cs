using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

  public class Checklist : MonoBehaviour
{
    [System.Serializable]
    public class TaskGroup
    {
        public string groupName;        // e.g. "Assemble Bones"
        public string[] tasks;          // e.g. {"Place Humerus", "Place Tibia"}
        [HideInInspector] public HashSet<string> completedTasks = new HashSet<string>();
    }

    public TaskGroup[] taskGroups;      // Multiple groups of tasks
    public Toggle[] toggles;            // One toggle per group (optional)
    private int currentGroup = 0;       // Tracks current progress group

    void Start()
    {
        // Reset all toggles to unchecked
        foreach (var toggle in toggles)
        {
            toggle.isOn = false;
        }
    }

    // ✅ Call this when user tries a task
    public bool TryDoTask(string taskName)
    {
        if (currentGroup >= taskGroups.Length)
        {
            Debug.Log("🎉 All task groups completed!");
            return false;
        }

        TaskGroup group = taskGroups[currentGroup];

        // Check if task exists in current group
        foreach (var t in group.tasks)
        {
            if (t == taskName)
            {
                group.completedTasks.Add(taskName);
                Debug.Log("✅ Completed task: " + taskName + " (" + group.completedTasks.Count + "/" + group.tasks.Length + ")");

                // Check if all tasks in this group are done
                if (group.completedTasks.Count == group.tasks.Length)
                {
                    CompleteGroup(currentGroup);
                }

                return true;
            }
        }

        Debug.Log("⛔ Task not allowed yet. Current group: " + group.groupName);
        return false;
    }

    public void DoTaskFromEvent(string taskName)
{
    TryDoTask(taskName);
}

    void CompleteGroup(int index)
    {
        if (index < toggles.Length)
        {
            toggles[index].isOn = true;
        }

        Debug.Log("🎯 Group completed: " + taskGroups[index].groupName);
        currentGroup++;

        if (currentGroup >= taskGroups.Length)
        {
            Debug.Log("🏁 All checklist stages complete!");
        }
        else
        {
            Debug.Log("➡️ Next group: " + taskGroups[currentGroup].groupName);
        }
    }

    // Optional helpers
    public bool AllGroupsDone()
    {
        return currentGroup >= taskGroups.Length;
    }

    public string GetNextGroupName()
    {
        return currentGroup < taskGroups.Length ? taskGroups[currentGroup].groupName : "All tasks complete!";
    }
}
