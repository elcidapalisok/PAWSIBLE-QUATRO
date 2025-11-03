using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Checklist : MonoBehaviour

{

    [System.Serializable]
 
    
    public class TaskGroup
    {
        public string groupName;
        public string[] tasks;
       public HashSet<string> completedTasks = new HashSet<string>();
       public int nextTaskIndex = 0;
        public float completionTime = 0f; // ⏱ save total time spent
    }
    public DrPawsPathWalker_Storymode drPawsPathWalker;
    public TaskGroup[] taskGroups;
    public Toggle[] toggles;
    public Animator animator;
    private int currentGroup = 0;

    [Header("Timer Reference")]
    public AssessmentTimer assessmentTimer; // 👈 connect this in Inspector
   [Header("UI Result Summary")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;     // ✅ Summary only
    public TextMeshProUGUI totalTimeText;
    void Start()
    {
        foreach (var toggle in toggles)
            toggle.isOn = false;

        if (assessmentTimer != null)
            assessmentTimer.StartTimer(); // start immediately or on first group
    }

    public bool TryDoTask(string taskName)
    {
        if (currentGroup >= taskGroups.Length)
        {
            Debug.Log("🎉 All task groups completed!");
            return false;
        }

        TaskGroup group = taskGroups[currentGroup];

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

    void CompleteGroup(int index)
    {
        if (index < toggles.Length)
            toggles[index].isOn = true;

        // ⏱ Save time taken for this group
        if (assessmentTimer != null)
        {
            taskGroups[index].completionTime = assessmentTimer.GetElapsedTime();
            Debug.Log($"⏱ Time taken for {taskGroups[index].groupName}: {taskGroups[index].completionTime:F2} seconds");

            // Reset timer for next group
            assessmentTimer.ResetTimer();
            assessmentTimer.StartTimer();
        }

        Debug.Log("🎯 Group completed: " + taskGroups[index].groupName);
        currentGroup++;

        if (currentGroup >= taskGroups.Length)
        {
            Debug.Log("🏁 All checklist stages complete!");
            if (assessmentTimer != null)
                assessmentTimer.StopTimer();
            ShowResults();
                 
        }
        else
        {
            Debug.Log("➡️ Next group: " + taskGroups[currentGroup].groupName);
        }
    }

    // Helper functions
    public bool AllGroupsDone() => currentGroup >= taskGroups.Length;
    public string GetNextGroupName() => currentGroup < taskGroups.Length ? taskGroups[currentGroup].groupName : "All tasks complete!";
    public int GetCurrentGroupIndex() => currentGroup;
    public bool HasGroupBeenCompleted(string groupName)
    {
        foreach (var group in taskGroups)
            if (group.groupName == groupName)
                return group.completedTasks.Count == group.tasks.Length;
        return false;
    }
  void ShowResults()
    {
        if (resultPanel == null || resultText == null || totalTimeText == null)
        {
            Debug.LogWarning("⚠ Result UI not assigned!");
            return;
        }

        resultPanel.SetActive(true);

        float totalTime = 0f;
        string summary = "<align=\"center\"><b>*Assessment Summary*</b>\n\n</align>";

        foreach (var group in taskGroups)
        {
            summary += $"<b>{group.groupName}:</b> <color=#FF0000>{FormatTime(group.completionTime)}</color>\n\n";
            totalTime += group.completionTime;
        }

        // ✅ Apply group summary only
        resultText.text = summary;

        // ✅ Total Time on another UI Text (no alignment inside code)
        totalTimeText.text =
            $"<b>Total Time:</b>\n" +
            $"<color=#FF0000>{FormatTime(totalTime)}</color>";
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}
