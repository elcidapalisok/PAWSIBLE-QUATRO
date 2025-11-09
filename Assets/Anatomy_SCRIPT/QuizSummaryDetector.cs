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
    public TMP_Text summaryText;

    [Header("Scoring")]
    public int pointsPerCorrect = 10;
    public int pointsPerWrong = -5;

    private List<string> correctTags = new List<string>()
    {
       "Liver", "Penis", "Heart"
    };

    private List<string> detectedTags = new List<string>();
    private List<string> wrongTags = new List<string>(); // Track wrong objects
    private int score = 0;

    void Start()
    {
        Debug.Log("=== QUIZ DETECTOR STARTED ===");
        summaryPanel.SetActive(false);
        SetupColliders();
    }

    void SetupColliders()
    {
        // Set up bones table collider
        if (bonesTable != null)
        {
            BoxCollider collider = bonesTable.GetComponent<BoxCollider>();
            if (collider == null) collider = bonesTable.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = Vector3.one * 3f; // Larger size
            Debug.Log($"Bones table collider setup: {collider.size}");
        }

        // Set up visceral freezer collider
        if (visceralFreezer != null)
        {
            BoxCollider collider = visceralFreezer.GetComponent<BoxCollider>();
            if (collider == null) collider = visceralFreezer.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = Vector3.one * 3f; // Larger size
            Debug.Log($"Visceral freezer collider setup: {collider.size}");
        }

        Debug.Log("Colliders setup complete");
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
                
                if (detectedTags.Count >= correctTags.Count)
                {
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
            }
            else
            {
                Debug.Log($"Already counted wrong: {other.tag}");
            }
        }
    }



    void ShowSummary()
    {
        if (summaryPanel == null || summaryText == null)
        {
            Debug.LogError("UI elements not assigned!");
            return;
        }

        summaryPanel.SetActive(true);
        
        // Calculate percentage
        float percentage = (float)detectedTags.Count / correctTags.Count * 100f;
        
        string result = $"QUIZ COMPLETE!\n";
        result += "====================\n";
        result += $"SCORE: {score} points\n";
        result += $"ACCURACY: {percentage:F1}%\n\n";
        
        result += $"Correct Objects: {detectedTags.Count}/{correctTags.Count}\n";
        foreach (string tag in correctTags)
        {
            string status = detectedTags.Contains(tag) ? "✓ FOUND" : "✗ MISSING";
            result += $"- {tag}: {status}\n";
        }
        
        result += $"\nWrong Objects: {wrongTags.Count}\n";
        if (wrongTags.Count > 0)
        {
            foreach (string wrongTag in wrongTags)
            {
                result += $"- {wrongTag} (penalty)\n";
            }
        }
        else
        {
            result += "- None! Great job! 🎉\n";
        }

        result += $"\nBreakdown:\n";
        result += $"+{detectedTags.Count * pointsPerCorrect} from correct answers\n";
        if (wrongTags.Count > 0)
        {
            result += $"{wrongTags.Count * pointsPerWrong} from wrong answers\n";
        }

        summaryText.text = result;
        Debug.Log("Summary shown!");
    }

    // Public method to manually show summary
    public void ManualShowSummary()
    {
        ShowSummary();
    }

    // Public method to reset the quiz
    public void ResetQuiz()
    {
        detectedTags.Clear();
        wrongTags.Clear();
        score = 0;
        summaryPanel.SetActive(false);
        Debug.Log("Quiz reset!");
    }

    // Get current score for other scripts
    public int GetCurrentScore()
    {
        return score;
    }

    // Get detection progress
    public string GetProgress()
    {
        return $"{detectedTags.Count}/{correctTags.Count}";
    }
}