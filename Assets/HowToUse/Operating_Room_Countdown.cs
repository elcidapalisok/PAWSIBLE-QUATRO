using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using System; // for timeline connection

public class AssessmentTimer : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI timerText;

    [Header("Control Options")]
    public bool startOnEnable = false;
    public PlayableDirector timelineDirector;

    private float elapsedTime;
    private bool isRunning = false;

    void OnEnable()
    {
        if (startOnEnable)
            StartTimer();
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        timerText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    // Public controls
    public void StartTimer()
    {
        isRunning = true;
        elapsedTime = 0f;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerUI();
    }

    // Optional: Called when timeline finishes
    public void OnTimelineEnd(PlayableDirector director)
    {
        StopTimer();
    }

 public float GetElapsedTime()
{
    return elapsedTime;
}
}
