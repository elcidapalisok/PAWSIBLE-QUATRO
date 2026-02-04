using TMPro;
using UnityEngine;

public class ScoreSummaryUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text accuracyText;
    public TMP_Text timeText;
    public TMP_Text mistakesText;
    public TMP_Text stepsDoneText;

    [Header("Star Rating")]
    public StarRatingUI ratingUI;

    private bool bound = false;

    private void OnEnable()
    {
        TryBind();
        Refresh();
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void Update()
    {
        // If ScoreManager spawns later, keep trying until we bind.
        if (!bound)
            TryBind();
    }

    private void TryBind()
    {
        if (bound) return;
        if (ScoreManager.Instance == null) return;

        ScoreManager.Instance.OnScoreChanged += Refresh;
        bound = true;
    }

    private void Unbind()
    {
        if (!bound) return;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= Refresh;

        bound = false;
    }

    public void Refresh()
    {
        if (ScoreManager.Instance == null)
            return;

        var report = ScoreManager.Instance.GetReport();

        if (accuracyText != null)
            accuracyText.text = " " + (report.accuracy01 * 100f).ToString("0") + "%";

        if (timeText != null)
            timeText.text = " " + report.formattedTime;

        if (mistakesText != null)
            mistakesText.text = " " + report.mistakes;

        if (stepsDoneText != null)
            stepsDoneText.text = " " + report.stepsDone + "/" + report.totalSteps;

        if (ratingUI != null)
            ratingUI.SetRating(report.ratingStars);
    }
}
