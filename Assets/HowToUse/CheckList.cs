using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Checklist : MonoBehaviour
{
    [System.Serializable]
    public class TaskGroup
    {
        public string groupName;
        public string[] tasks;
        [HideInInspector] public HashSet<string> completedTasks = new HashSet<string>();
        [HideInInspector] public int nextTaskIndex = 0; // Track the next allowed task in order
    }

    public TaskGroup[] taskGroups;
    public Toggle[] toggles;
    private int currentGroup = 0;

    void Start()
    {
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

        // Restrict task to the next one in order
        if (group.nextTaskIndex >= group.tasks.Length)
        {
            Debug.Log("✅ All tasks in this group already done.");
            return false;
        }

        string nextTask = group.tasks[group.nextTaskIndex];
        if (taskName == nextTask)
        {
            group.completedTasks.Add(taskName);
            group.nextTaskIndex++;
            Debug.Log("✅ Completed task: " + taskName + " (" + group.completedTasks.Count + "/" + group.tasks.Length + ")");

            // Check if all tasks in this group are done
            if (group.completedTasks.Count == group.tasks.Length)
            {
                CompleteGroup(currentGroup);
            }

            return true;
        }
        else
        {
            Debug.Log("⛔ Task not allowed yet. Complete '" + nextTask + "' first in group: " + group.groupName);
            return false;
        }
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

    public int GetCurrentGroupIndex()
    {
        return currentGroup;
    }

    public bool HasGroupBeenCompleted(string groupName)
    {
        foreach (var group in taskGroups)
        {
            if (group.groupName == groupName)
            {
                return group.completedTasks.Count == group.tasks.Length;
            }
        }
        return false;
    }
}
