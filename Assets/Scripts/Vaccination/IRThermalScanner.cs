using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class IRThermalScannerTrigger : MonoBehaviour
{
    [Header("UI (TMP Text)")]
    [SerializeField] private TMP_Text celsiusText;
    [SerializeField] private TMP_Text fahrenheitText;

    [Header("Detection Colliders (Clear Naming)")]
    [Tooltip("A) The dog's collider that must enter the scan volume (recommended).")]
    [SerializeField] private Collider dogTargetCollider;

    [Tooltip("B) The scan volume collider to use (should be THIS object's trigger collider). Leave empty to auto-use this collider.")]
    [SerializeField] private Collider scanVolumeCollider;

    [Header("Optional Fallback Filters (if dogTargetCollider is not assigned)")]
    [SerializeField] private string requiredDogTag = "Dog";
    [SerializeField] private LayerMask dogLayers = ~0;

    [Header("Temperature Range (Normal Dog Temp, Celsius)")]
    [SerializeField] private float minCelsius = 38.3f;
    [SerializeField] private float maxCelsius = 39.2f;

    [Header("Formatting")]
    [SerializeField] private int decimalPlaces = 1;
    [SerializeField] private string celsiusSuffix = "°C";
    [SerializeField] private string fahrenheitSuffix = "°F";

    [Header("Dialogue Gate (advance only at this stage)")]
    [SerializeField] private string dialogueSegmentName = "Injection";
    [SerializeField] private int dialogueLineIndex = 5;

    [Header("Feedback Spawn")]
    public Transform feedbackAnchor;
    public Vector3 feedbackOffset = new Vector3(0f, 0.15f, 0f);

    [Header("Wrong Feedback")]
    public bool showWrongFeedbackWhenNotAtStage = true;
    [Min(0f)] public float wrongFeedbackCooldown = 1.0f;

    [Header("Scan Behavior")]
    [SerializeField] private bool oneScanPerEnter = true;

    private bool hasScannedThisEnter = false;
    private float lastWrongTime = -999f;

    private void Awake()
    {
        if (scanVolumeCollider == null)
            scanVolumeCollider = GetComponent<Collider>();

        if (scanVolumeCollider != null && !scanVolumeCollider.isTrigger)
        {
            Debug.LogWarning("[IRThermalScannerTrigger] '" + scanVolumeCollider.name + "' was not a trigger. Setting isTrigger = true.");
            scanVolumeCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (oneScanPerEnter && hasScannedThisEnter) return;
        if (!IsDogMatch(other)) return;

        DoScanAndUIUpdate();

        DialogueManager dm = DialogueManager.Instance;
        if (dm == null) return;

        if (dm.IsAtStage(dialogueSegmentName, dialogueLineIndex))
        {
            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportCorrect(spawnPos);

            ScoreManager.Instance?.RegisterCorrect(dialogueSegmentName, dialogueLineIndex);

            dm.AdvanceDialogue();

            if (oneScanPerEnter)
                hasScannedThisEnter = true;

            return;
        }

        if (showWrongFeedbackWhenNotAtStage && Time.time - lastWrongTime >= wrongFeedbackCooldown)
        {
            lastWrongTime = Time.time;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportWrong(spawnPos);

            ScoreManager.Instance?.RegisterMistake(
                dm.GetCurrentSegmentName(),
                dm.GetCurrentLineIndex(),
                "Dog scanned with IR scanner at wrong stage"
            );
        }

        if (oneScanPerEnter)
            hasScannedThisEnter = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsDogMatch(other))
            hasScannedThisEnter = false;
    }

    private bool IsDogMatch(Collider other)
    {
        if (dogTargetCollider != null)
            return other == dogTargetCollider;

        bool layerOk = (dogLayers.value & (1 << other.gameObject.layer)) != 0;

        bool tagOk = true;
        if (!string.IsNullOrWhiteSpace(requiredDogTag))
            tagOk = other.CompareTag(requiredDogTag);

        return layerOk && tagOk;
    }

    private void DoScanAndUIUpdate()
    {
        float c = Random.Range(minCelsius, maxCelsius);
        float f = (c * 9f / 5f) + 32f;

        if (celsiusText != null)
            celsiusText.text = c.ToString("F" + decimalPlaces) + celsiusSuffix;

        if (fahrenheitText != null)
            fahrenheitText.text = f.ToString("F" + decimalPlaces) + fahrenheitSuffix;

        Debug.Log("[IRThermalScannerTrigger] Scan: " +
                  (celsiusText != null ? celsiusText.text : "null") +
                  " / " +
                  (fahrenheitText != null ? fahrenheitText.text : "null"));
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        Transform anchor = feedbackAnchor != null ? feedbackAnchor : transform;
        return anchor.position + feedbackOffset;
    }
}
