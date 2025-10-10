using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

public class triggerPoints : MonoBehaviour
{
    public PlayableDirector director;
    public List<Step> steps;
    public Checklist checklist; // Reference to Checklist script

    private bool gameStarted = false;


    void Start()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();

        StartCoroutine(EnableNextFrame());


    }

    IEnumerator EnableNextFrame()
    {
        yield return null;
        gameStarted = true;
    }

    [System.Serializable]
    public class Step
    {
        public string name;     // Example: "Wear Lab Coat"
        public float time;      // Timeline time
        public bool hasPlayed = false;
    }

    // Call this when player interacts with an object
    public void TryPlayStep(int index)
    {
        if (!gameStarted) return;
        if (index < 0 || index >= steps.Count) return;

        Step step = steps[index];

        // Check checklist first
        if (checklist.TryDoTask(step.name))
        {
            if (!step.hasPlayed)
            {
               
                step.hasPlayed = true;
                director.Stop();
                director.time = step.time;
                director.Play();

                Debug.Log($"🎬 Playing step {index}: {step.name}");
            }
        }
      
    }
}
