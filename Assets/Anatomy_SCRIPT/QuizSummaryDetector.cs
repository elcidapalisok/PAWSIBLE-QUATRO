using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class QuizSummaryDetector : MonoBehaviour
{
    [Header("Detection Zones")]
    public GameObject bonesTable;
    public GameObject visceralFreezer;

    [Header("UI Elements")]
    public GameObject summaryPanel;
    [Header("World Output Texts")]
    public TMP_Text summaryTitleText;
    public TMP_Text scoreText;

    public TMP_Text correctText;
    public TMP_Text wrongText;
    public TMP_Text breakdownTitleText;
    public TMP_Text breakdownDetailsText;

    [Header("Progress Bars")]
    public Image accuracyProgressBar;
    public Image timeScoreProgressBar;
    public TMP_Text accuracyPercentageText;
    public TMP_Text timeScorePercentageText;

    [Header("Scoring")]
    public int pointsPerCorrect = 10;
    public int pointsPerWrong = -5;
    public int maxTimeScore = 100; // Maximum time-based score
    public float timeWeight = 0.3f; // How much time affects final score (0-1)

    // Reference to your timer
    public AssessmentTimer timer;

    private List<string> correctTags = new List<string>()
    {
       "Humerus", "Pelvic", "Vertaebrae", "Liver", "Penis", "Heart"
    };

    private List<string> detectedTags = new List<string>();
    private List<string> wrongTags = new List<string>(); // Track wrong objects
    private int score = 0;
    private int timeBasedScore = 0;
    private int finalScore = 0;

    void Start()
    {
        Debug.Log("=== QUIZ DETECTOR STARTED ===");
        if (summaryPanel != null)
        {
            summaryPanel.SetActive(false);
        }

        // Initialize progress bars
        if (accuracyProgressBar != null)
            accuracyProgressBar.fillAmount = 0f;
        if (timeScoreProgressBar != null)
            timeScoreProgressBar.fillAmount = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"TRIGGER ENTERED: {other.gameObject.name} (Tag: {other.tag})");
        
        // Skip if tag is Untagged
        if (other.tag == "Untagged")
        {
            Debug.Log($"Ignored: {other.gameObject.name} has no tag");
            return;
        }

        if (correctTags.Contains(other.tag))
        {
            if (!detectedTags.Contains(other.tag))
            {
                detectedTags.Add(other.tag);
                score += pointsPerCorrect;
                Debug.Log($"✅ CORRECT: {other.tag} +{pointsPerCorrect}pts - Total: {detectedTags.Count}/{correctTags.Count} - Score: {score}");
                
                // Update accuracy progress bar in real-time
                UpdateAccuracyProgressBar();
                
                if (detectedTags.Count >= correctTags.Count)
                {
                    if (timer != null)
                        timer.StopTimer();
                    ShowSummary();
                }
            }
            else
            {
                Debug.Log($"Already detected: {other.tag}");
            }
        }
        else
        {
            // Wrong object detected
            if (!wrongTags.Contains(other.tag))
            {
                wrongTags.Add(other.tag);
                score += pointsPerWrong;
                Debug.Log($"❌ WRONG: {other.tag} {pointsPerWrong}pts - Score: {score}");
                
                // Update accuracy progress bar in real-time
                UpdateAccuracyProgressBar();
            }
            else
            {
                Debug.Log($"Already counted wrong: {other.tag}");
            }
        }
    }

    void UpdateAccuracyProgressBar()
    {
        if (accuracyProgressBar != null)
        {
            float accuracy = Mathf.Clamp(
                ((float)detectedTags.Count / correctTags.Count) * 100f -
                ((float)wrongTags.Count / correctTags.Count) * 50f,
                0f, 100f
            );
            
            accuracyProgressBar.fillAmount = accuracy / 100f;
            
            if (accuracyPercentageText != null)
                accuracyPercentageText.text = $"{accuracy:F1}%";
        }
    }

    int CalculateTimeBasedScore()
    {
        if (timer == null) return maxTimeScore;
        
        float elapsedTime = timer.GetElapsedTime();
        
        // Example: More time = lower score, but never below 0
        // You can adjust this formula based on your needs
        float timeScore = Mathf.Max(0, maxTimeScore - (elapsedTime / 60f) * 10f);
        
        return Mathf.RoundToInt(timeScore);
    }

    void UpdateTimeScoreProgressBar(int timeScore)
    {
        if (timeScoreProgressBar != null)
        {
            float normalizedScore = (float)timeScore / maxTimeScore;
            timeScoreProgressBar.fillAmount = normalizedScore;
            
            if (timeScorePercentageText != null)
                timeScorePercentageText.text = $"{timeScore} pts";
        }
    }

    public void ShowSummary()
    {
        // Show Objects if they were hidden
        summaryTitleText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
 
        correctText.gameObject.SetActive(true);
        wrongText.gameObject.SetActive(true);
        breakdownTitleText.gameObject.SetActive(true);
        breakdownDetailsText.gameObject.SetActive(true);
        
        if (summaryPanel != null)
            summaryPanel.SetActive(true);

        // Calculate scores
        float accuracyPercentage = Mathf.Clamp(
            ((float)detectedTags.Count / correctTags.Count) * 100f -
            ((float)wrongTags.Count / correctTags.Count) * 50f,
            0f, 100f
        );

        timeBasedScore = CalculateTimeBasedScore();
        
        // Calculate final score (combination of accuracy and time)
        finalScore = Mathf.RoundToInt(
            (score * (1f - timeWeight)) + 
            (timeBasedScore * timeWeight)
        );

        // Update UI
        summaryTitleText.text = "Mastery Evaluation";
        scoreText.text = $"Final Score: {finalScore} pts";
      

        // ✅ Show item names for correct
        string correctList = string.Join(", ", detectedTags);
        correctText.text = $"<color=#00FF00>Correct:</color> {correctList}";

        // ✅ Show item names for wrong (RED)
        if (wrongTags.Count > 0)
        {
            string wrongList = string.Join(", ", wrongTags);
            wrongText.text = $"<color=#FF0000>Wrong:</color> {wrongList}";
        }
        else
        {
            wrongText.text = "<color=#FF0000>Wrong:</color> None";
        }

        breakdownTitleText.text = "Summary:";

        string details = $"+{detectedTags.Count * pointsPerCorrect} correct points\n";
        details += $"+{timeBasedScore} time bonus\n";
        if (wrongTags.Count > 0)
            details += $"{wrongTags.Count * pointsPerWrong} penalty\n";

        breakdownDetailsText.text = details;

        // Update progress bars
        UpdateAccuracyProgressBar();
        UpdateTimeScoreProgressBar(timeBasedScore);

        Debug.Log("Separated Summary shown!");
    }

    // Public method to manually show summary
    public void ManualShowSummary()
    {
        if (timer != null)
            timer.StopTimer();
        ShowSummary();
    }

    // Public method to reset the quiz
    public void ResetQuiz()
    {
        detectedTags.Clear();
        wrongTags.Clear();
        score = 0;
        timeBasedScore = 0;
        finalScore = 0;
        
        if (summaryPanel != null)
            summaryPanel.SetActive(false);
            
        if (accuracyProgressBar != null)
            accuracyProgressBar.fillAmount = 0f;
        if (timeScoreProgressBar != null)
            timeScoreProgressBar.fillAmount = 0f;
            
        if (timer != null)
        {
            timer.ResetTimer();
            timer.StartTimer();
        }
        
        Debug.Log("Quiz reset!");
    }

    // Get current score for other scripts
    public int GetCurrentScore()
    {
        return finalScore;
    }

    // Get detection progress
    public string GetProgress()
    {
        return $"{detectedTags.Count}/{correctTags.Count}";
    }
}