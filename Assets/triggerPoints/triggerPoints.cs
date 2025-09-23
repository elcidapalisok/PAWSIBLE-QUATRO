using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

public class triggerPoints : MonoBehaviour
{
    public PlayableDirector director;
    public List<Step> steps;

    // Prevent auto-trigger at scene start
    private bool gameStarted = false;

    void Start()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();

        // Delay enabling gameplay by one frame to avoid Timeline auto-evaluation
        StartCoroutine(EnableNextFrame());
    }

    IEnumerator EnableNextFrame()
    {
        yield return null; // wait one frame
        gameStarted = true;
    }

    [System.Serializable]
    public class Step
    {
        public string name;
        public float time;
        public bool hasPlayed = false;
    }

    // Core timeline triggering method
    public void PlayStepIndex(int index)
    {
        if (!gameStarted) return; // ignore calls before gameplay starts
        if (index < 0 || index >= steps.Count) return;

        Step step = steps[index];

        if (!step.hasPlayed)
        {
            step.hasPlayed = true;
            director.Stop();
            director.time = step.time;
            director.Play();
            Debug.Log($"Playing step {index}: {step.name}");
        }
    }
}
