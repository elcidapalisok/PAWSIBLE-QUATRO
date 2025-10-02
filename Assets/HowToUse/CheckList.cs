using UnityEngine;
using UnityEngine.UI;
public class Checklist : MonoBehaviour
{
    public string[] tasks;       // Example: {"Wear Lab Coat", "Grab Scalpel", "Pick Femur"}
    private int currentTask = 0; // Keeps track of progress
    public Toggle[] toggles;
    // Call this when user tries a task
    public bool TryDoTask(string taskName)
    {
        if (currentTask < tasks.Length && tasks[currentTask] == taskName)
        {
            Debug.Log("✅ Completed number: " + currentTask);
            Debug.Log("✅ Completed: " + taskName);
            currentTask++; // Move to next task
            return true;   // Allowed
        }
        else
        {
            Debug.Log("⛔ Can't do " + taskName + " yet. Next required: " + tasks[currentTask]);
            return false;  // Not allowed
        }
    }
void Start()
{
    // Reset all toggles to unchecked
    foreach (var toggle in toggles)
    {
        toggle.isOn = false;
    }
}
public void CompleteTaskAtIndex(int index)
{
    if (index >= 0 && index < toggles.Length)
    {
        toggles[index].isOn = true;
        Debug.Log("✅ Timeline marked task: " + tasks[index]);
    }
    else
    {
        Debug.LogWarning("⚠️ Invalid task index: " + index);
    }
}


    // Optional: check if all done
    public bool AllTasksDone()
    {
        return currentTask >= tasks.Length;
    }

    // Get the next required task
    public string GetNextTask()
    {
        return currentTask < tasks.Length ? tasks[currentTask] : "All tasks complete!";
    }
}
