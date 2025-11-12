using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class CutsceneDialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public CanvasGroup subtitleCanvas;
    public Button nextButton;
    public Button prevButton;
    public Button skipButton;

    [Header("Audio Settings")]
    public AudioSource voiceSource;
    [Tooltip("Relative path inside Resources folder, e.g. 'Audio/Vaccination_Cutscene1'")]
    public string audioFolderPath = "Audio/Vaccination_Cutscene1";

    [Header("Typewriter Settings")]
    [Tooltip("Typing speed per character. Set lower for faster text.")]
    public float typingSpeed = 0.001f; // almost instant typing

    [Header("Auto Advance Settings")]
    [Tooltip("Delay before automatically showing next line after voice ends")]
    public float autoAdvanceDelay = 0.3f; // short delay before next line

    [Header("Dialogue Lines")]
    [TextArea(2, 10)]
    public List<string> dialogueLines = new List<string>();

    [Header("Scene Transition")]
    [Tooltip("Scene to load after cutscene ends")]
    public string nextSceneName = "StoryMode_Vaccine";

    private int currentLine = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    void Start()
    {
        // Optional: Button Listeners
        if (nextButton != null) nextButton.onClick.AddListener(ShowNextLine);
        if (prevButton != null) prevButton.onClick.AddListener(ShowPrevLine);
        if (skipButton != null) skipButton.onClick.AddListener(SkipCutscene);

        // Hide subtitles at start
        if (dialogueText != null)
            dialogueText.gameObject.SetActive(false);
        if (subtitleCanvas != null)
            subtitleCanvas.alpha = 0f;
    }

    // -----------------------------
    // TIMELINE SIGNAL TRIGGER
    // -----------------------------
    public void TriggerDialogueLine(int index)
    {
        ShowLine(index);
    }

    public void ShowLine(int index)
    {
        if (index < 0 || index >= dialogueLines.Count) return;

        currentLine = index;

        // Stop any previous typing
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(dialogueLines[index]));

        // Play corresponding voice
        PlayVoiceForCurrentLine();
    }

    // -----------------------------
    // TYPEWRITER EFFECT
    // -----------------------------
    IEnumerator TypeText(string text)
    {
        if (subtitleCanvas != null) subtitleCanvas.alpha = 1f;
        dialogueText.gameObject.SetActive(true);
        dialogueText.text = "";
        isTyping = true;

        // instantly show entire text (no visible delay)
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // Wait for the voice line to finish
        if (voiceSource != null && voiceSource.clip != null)
        {
            while (voiceSource.isPlaying)
                yield return null;
        }

        // Wait briefly before advancing
        yield return new WaitForSeconds(autoAdvanceDelay);

        // Auto advance if not the last line
        if (currentLine + 1 < dialogueLines.Count)
        {
            ShowLine(currentLine + 1);
        }
        else
        {
            EndCutscene();
        }
    }

    // -----------------------------
    // AUDIO HANDLING
    // -----------------------------
    void PlayVoiceForCurrentLine()
    {
        if (voiceSource == null) return;

        string fileName = $"{currentLine:D2}"; // e.g., 00, 01
        string fullPath = Path.Combine(audioFolderPath, fileName);

        AudioClip clip = Resources.Load<AudioClip>(fullPath);
        if (clip != null)
        {
            voiceSource.clip = clip;
            voiceSource.Play();
        }
        else
        {
            Debug.LogWarning($"CutsceneDialogueManager: Missing audio clip at {fullPath}");
        }
    }

    // -----------------------------
    // BUTTONS
    // -----------------------------
    public void ShowNextLine()
    {
        // Skip typing instantly if still animating
        if (isTyping)
        {
            isTyping = false;
            dialogueText.text = dialogueLines[currentLine];
            return;
        }

        if (currentLine + 1 < dialogueLines.Count)
            ShowLine(currentLine + 1);
        else
            EndCutscene();
    }

    public void ShowPrevLine()
    {
        if (currentLine - 1 >= 0)
            ShowLine(currentLine - 1);
    }

    public void SkipCutscene()
    {
        EndCutscene();
    }

    // -----------------------------
    // END CUTSCENE
    // -----------------------------
    void EndCutscene()
    {
        // Stop audio
        if (voiceSource != null && voiceSource.isPlaying)
            voiceSource.Stop();

        // Fade out before loading next scene
        if (subtitleCanvas != null)
            StartCoroutine(FadeOutAndLoadNextScene());
        else
            LoadNextScene();
    }

    IEnumerator FadeOutAndLoadNextScene()
    {
        float t = 0f;
        float duration = 0.5f;
        float startAlpha = subtitleCanvas.alpha;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            subtitleCanvas.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        dialogueText.gameObject.SetActive(false);
        LoadNextScene();
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"[CutsceneDialogueManager] Loading next scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("[CutsceneDialogueManager] No next scene defined.");
        }
    }
}
