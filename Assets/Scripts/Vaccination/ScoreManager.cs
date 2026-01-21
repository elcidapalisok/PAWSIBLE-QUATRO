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
        public int points = 200;
    }

    public static ScoreManager Instance { get; private set; }

    [Header("Steps included in scoring (gated steps only)")]
    public List<ScoredStep> scoredSteps = new List<ScoredStep>();

    [Header("Penalty")]
    public int mistakePenalty = 200;

    // tracking
    private readonly HashSet<string> completedStepKeys = new HashSet<string>();
    private int mistakes = 0;
    private int totalScore = 0;

    public event Action OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private string Key(string segment, int line) => $"{DialogueManager.NormalizeSegmentKey(segment)}:{line}";

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
        if (!IsStepScored(segment, line)) return;

        string k = Key(segment, line);
        if (completedStepKeys.Contains(k)) return; // no double count

        completedStepKeys.Add(k);

        int pts = GetPoints(segment, line);
        totalScore += pts;

        OnScoreChanged?.Invoke();
        Debug.Log($"[SCORE] Correct step registered: {segment}:{line} | Score={totalScore}");

    }

    public void RegisterMistake(string segment, int line, string reason = "")
    {
        // mistakes can be counted even if the attempted step isn’t in scoredSteps
        // but you can restrict this if you want.
        mistakes++;
        totalScore -= mistakePenalty;

        OnScoreChanged?.Invoke();
        Debug.Log($"[SCORE] Mistake registered at {segment}:{line} | Score={totalScore}");

    }

    private int GetPoints(string segment, int line)
    {
        string k = Key(segment, line);
        for (int i = 0; i < scoredSteps.Count; i++)
        {
            if (Key(scoredSteps[i].segmentName, scoredSteps[i].lineIndex) == k)
                return scoredSteps[i].points;
        }
        return 0;
    }

    public ScoreReport GetReport()
    {
        int totalSteps = scoredSteps.Count;
        int stepsDone = completedStepKeys.Count;
        int stepsRemaining = Mathf.Max(0, totalSteps - stepsDone);

        int correct = stepsDone;
        int wrong = mistakes;

        float accuracy = (correct + wrong) > 0 ? (float)correct / (correct + wrong) : 1f;
        float procedure = totalSteps > 0 ? (float)stepsDone / totalSteps : 1f;

        return new ScoreReport
        {
            mistakes = mistakes,
            stepsDone = stepsDone,
            stepsRemaining = stepsRemaining,
            totalSteps = totalSteps,
            totalScore = totalScore,
            accuracy01 = accuracy,
            procedure01 = procedure
        };
    }

    [Serializable]
    public struct ScoreReport
    {
        public int mistakes;
        public int stepsDone;
        public int stepsRemaining;
        public int totalSteps;
        public int totalScore;
        public float accuracy01;
        public float procedure01;
    }
}
