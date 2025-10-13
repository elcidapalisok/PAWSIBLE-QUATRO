using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using System.Collections.Generic;

public class TriggerPoints : MonoBehaviour
{
    [Header("Timeline Director")]
    public PlayableDirector director;

    [Header("Checklist Reference")]
    public Checklist checklist; // Reference to your Checklist script

    [Header("Steps List")]
    public List<Step> steps = new List<Step>();

    private bool gameStarted = false;

    void Start()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();

        StartCoroutine(EnableNextFrame());
    }

    // Waits one frame before enabling the game logic
    IEnumerator EnableNextFrame()
    {
        yield return null;
        gameStarted = true;
    }

    [System.Serializable]
    public class Step
    {
        public string name;   // Example: "Wear Lab Coat"
        public float time;    // Timeline time (in seconds)
        [HideInInspector] public bool hasPlayed = false;
    }

    /// <summary>
    /// Plays the timeline at the step’s specified time if the corresponding checklist item is completed.
    /// </summary>
    /// <param name="index">Index of the step in the list.</param>
    public void TryPlayStep(int index)
    {
        if (!gameStarted)
        {
            Debug.LogWarning("⏳ Game not started yet!");
            return;
        }

        if (index < 0 || index >= steps.Count)
        {
            Debug.LogWarning($"⚠ Invalid step index: {index}");
            return;
        }

        Step step = steps[index];

        if (checklist == null)
        {
            Debug.LogError("❌ Checklist reference missing!");
            return;
        }

        // Check if the task can be performed
        if (checklist.TryDoTask(step.name))
        {
            if (!step.hasPlayed)
            {
                step.hasPlayed = true;

                if (director != null)
                {
                    director.Stop();
                    director.time = step.time;
                    director.Play();
                    Debug.Log($"🎬 Playing step {index}: {step.name}");
                }
                else
                {
                    Debug.LogError("❌ No PlayableDirector assigned!");
                }
            }
            else
            {
                Debug.Log($"⏩ Step '{step.name}' already played, skipping...");
            }
        }
        else
        {
            Debug.Log($"🚫 Checklist task '{step.name}' not yet completed.");
        }
    }
}
