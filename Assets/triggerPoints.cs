using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
public class triggerPoints : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    PlayableDirector director;
    public List<Step> steps;
    void Start()
    {
        director = GetComponent<PlayableDirector>();
    }
    [System.Serializable]
    public class Step
    {
        public string name;
        public float time;
        public bool hasPlayed = false;
    }
    // Update is called once per frame
    public void PlayStepIndex(int index)
    {
        Step step = steps[index];

        if (!step.hasPlayed)
        {
            step.hasPlayed = true;

            director.Stop();
            director.time = step.time;
            director.Play();
        }
    }
}
