using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Serializable]
    public class ScoredStep
    {
        public string segmentName;
        public int lineIndex;
    }

    public static ScoreManager Instance { get; private set; }

    [Header("Steps included in scoring (gated steps only)")]
    public List<ScoredStep> scoredSteps = new List<ScoredStep>();

    [Header("Timer")]
    public bool autoStartTimer = true;
    public bool stopTimerWhenComplete = true;

    [Tooltip("If true, timer keeps running even if Time.timeScale is 0.")]
    public bool useUnscaledTime = false;

    [Header("Rating (1 to 5 stars)")]
    public float targetTimeSeconds = 180f;
    public float maxTimeSeconds = 600f;
    public int mistakesForZero = 6;

    [Range(0f, 1f)] public float weightAccuracy = 0.35f;
    [Range(0f, 1f)] public float weightProcedure = 0.35f;
    [Range(0f, 1f)] public float weightTime = 0.15f;
    [Range(0f, 1f)] public float weightMistakes = 0.15f;

    private readonly HashSet<string> completedStepKeys = new HashSet<string>();
    private int mistakes = 0;

    private float elapsedSeconds = 0f;
    private bool timerRunning = false;

    private float uiTickTimer = 0f;
    private const float uiTickInterval = 0.25f; // update UI 4x per second

    public event Action OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (autoStartTimer)
            StartTimer();

        NotifyChanged();
    }

    private void Update()
    {
        if (timerRunning)
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsedSeconds += dt;

            uiTickTimer += dt;
            if (uiTickTimer >= uiTickInterval)
            {
                uiTickTimer = 0f;
                NotifyChanged();
            }

            if (stopTimerWhenComplete && GetTotalSteps() > 0 && GetStepsDone() >= GetTotalSteps())
                StopTimer();
        }
    }

    private void NotifyChanged()
    {
        if (OnScoreChanged != null)
            OnScoreChanged.Invoke();
    }

    private string Key(string segment, int line)
    {
        return DialogueManager.NormalizeSegmentKey(segment) + ":" + line;
    }

    public void StartTimer()
    {
        timerRunning = true;
        NotifyChanged();
    }

    public void StopTimer()
    {
        timerRunning = false;
        NotifyChanged();
    }

    public void ResetRun(bool restartTimer = true)
    {
        completedStepKeys.Clear();
        mistakes = 0;
        elapsedSeconds = 0f;
        uiTickTimer = 0f;
        timerRunning = restartTimer;
        NotifyChanged();
    }

    public bool IsStepScored(string segment, int line)
    {
        string k = Key(segment, line);
        for (int i = 0; i < scoredSteps.Count; i++)
        {
            if (Key(scoredSteps[i].segmentName, scoredSteps[i].lineIndex) == k)
                return true;
        }
        return false;
    }

    public void RegisterCorrect(string segment, int line)
    {
        if (!IsStepScored(segment, line))
            return;

        string k = Key(segment, line);

        if (completedStepKeys.Contains(k))
            return;

        completedStepKeys.Add(k);

        NotifyChanged();
        Debug.Log("[SCORE] Correct step registered: " + segment + ":" + line);
    }

    public void RegisterMistake(string segment, int line, string reason = "")
    {
        mistakes++;

        NotifyChanged();

        if (string.IsNullOrEmpty(reason))
            Debug.Log("[SCORE] Mistake registered at " + segment + ":" + line);
        else
            Debug.Log("[SCORE] Mistake registered at " + segment + ":" + line + " | " + reason);
    }

    public int GetMistakes()
    {
        return mistakes;
    }

    public float GetElapsedSeconds()
    {
        return elapsedSeconds;
    }

    public string GetElapsedTimeFormatted()
    {
        int total = Mathf.FloorToInt(elapsedSeconds);
        int hours = total / 3600;
        int minutes = (total % 3600) / 60;
        int seconds = total % 60;
        return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public int GetTotalSteps()
    {
        return scoredSteps != null ? scoredSteps.Count : 0;
    }

    public int GetStepsDone()
    {
        return completedStepKeys.Count;
    }

    public int GetStepsRemaining()
    {
        return Mathf.Max(0, GetTotalSteps() - GetStepsDone());
    }

    public float GetAccuracy01()
    {
        int correct = GetStepsDone();
        int wrong = mistakes;

        if (correct + wrong <= 0)
            return 1f;

        return (float)correct / (float)(correct + wrong);
    }

    public float GetProcedure01()
    {
        int total = GetTotalSteps();
        if (total <= 0) return 1f;
        return (float)GetStepsDone() / (float)total;
    }

    private float GetTimeScore01()
    {
        if (maxTimeSeconds <= 0f) return 1f;

        float t = elapsedSeconds;

        if (t <= targetTimeSeconds) return 1f;

        float denom = Mathf.Max(0.0001f, maxTimeSeconds - targetTimeSeconds);
        float normalized = 1f - ((t - targetTimeSeconds) / denom);
        return Mathf.Clamp01(normalized);
    }

    private float GetMistakeScore01()
    {
        if (mistakesForZero <= 0) return 1f;

        float normalized = 1f - ((float)mistakes / (float)mistakesForZero);
        return Mathf.Clamp01(normalized);
    }

    public float GetRating01()
    {
        float a = GetAccuracy01();
        float p = GetProcedure01();
        float t = GetTimeScore01();
        float m = GetMistakeScore01();

        float sum =
            (a * weightAccuracy) +
            (p * weightProcedure) +
            (t * weightTime) +
            (m * weightMistakes);

        return Mathf.Clamp01(sum);
    }

    public int GetRatingStars()
    {
        float r = GetRating01();

        if (r >= 0.90f) return 5;
        if (r >= 0.75f) return 4;
        if (r >= 0.55f) return 3;
        if (r >= 0.35f) return 2;
        return 1;
    }

    public ScoreReport GetReport()
    {
        ScoreReport report = new ScoreReport();
        report.mistakes = mistakes;
        report.stepsDone = GetStepsDone();
        report.stepsRemaining = GetStepsRemaining();
        report.totalSteps = GetTotalSteps();
        report.accuracy01 = GetAccuracy01();
        report.procedure01 = GetProcedure01();
        report.elapsedSeconds = elapsedSeconds;
        report.rating01 = GetRating01();
        report.ratingStars = GetRatingStars();
        report.formattedTime = GetElapsedTimeFormatted();
        return report;
    }

    [Serializable]
    public struct ScoreReport
    {
        public int mistakes;
        public int stepsDone;
        public int stepsRemaining;
        public int totalSteps;

        public float accuracy01;
        public float procedure01;

        public float elapsedSeconds;
        public string formattedTime;

        public float rating01;
        public int ratingStars;
    }
}
