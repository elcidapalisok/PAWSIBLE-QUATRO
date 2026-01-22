using TMPro;
using UnityEngine;

public class ScoreSummaryUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text accuracyText;
    public TMP_Text procedureText;
    public TMP_Text mistakesText;
    public TMP_Text stepsDoneText;
    public TMP_Text totalScoreText;

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (ScoreManager.Instance == null)
            return;

        var report = ScoreManager.Instance.GetReport();

        accuracyText.text = $"Accuracy: {(report.accuracy01 * 100f):0}%";
        procedureText.text = $"Procedure: {(report.procedure01 * 100f):0}%";
        mistakesText.text = $"Mistakes: {report.mistakes}";
        stepsDoneText.text = $"Steps Done: {report.stepsDone}/{report.totalSteps}";
        totalScoreText.text = $"Total Score: {report.totalScore} pts";
    }
}
