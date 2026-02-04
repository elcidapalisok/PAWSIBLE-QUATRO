using TMPro;
using UnityEngine;

public class ScoreClockUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text timerText;

    [Header("Timer Settings")]
    [SerializeField] private bool autoStart = true;

    private float elapsedSeconds;
    private bool running;

    private void Awake()
    {
        if (timerText == null)
        {
            Debug.LogWarning(name + ": Timer Text is not assigned.");
        }
    }

    private void Start()
    {
        elapsedSeconds = 0f;
        running = autoStart;
        UpdateTimerText();
    }

    private void Update()
    {
        if (!running) return;

        elapsedSeconds += Time.deltaTime;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText == null) return;

        int total = Mathf.FloorToInt(elapsedSeconds);
        int hours = total / 3600;
        int minutes = (total % 3600) / 60;
        int seconds = total % 60;

        if (hours > 0)
            timerText.text = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
        else
            timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void StartClock()
    {
        running = true;
    }

    public void StopClock()
    {
        running = false;
    }

    public void ResetClock()
    {
        elapsedSeconds = 0f;
        UpdateTimerText();
    }

    public float GetElapsedSeconds()
    {
        return elapsedSeconds;
    }
}
