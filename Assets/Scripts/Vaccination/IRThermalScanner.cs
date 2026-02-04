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

    [Header("Scan Behavior")]
    [SerializeField] private bool oneScanPerEnter = true;

    private bool hasScannedThisEnter = false;

    private void Awake()
    {
        // Auto-assign scan collider to THIS object's collider if not set
        if (scanVolumeCollider == null)
            scanVolumeCollider = GetComponent<Collider>();

        // Enforce: scan volume must be trigger
        if (scanVolumeCollider != null && !scanVolumeCollider.isTrigger)
        {
            Debug.LogWarning($"[IRThermalScannerTrigger] '{scanVolumeCollider.name}' was not a trigger. Setting isTrigger = true.");
            scanVolumeCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (oneScanPerEnter && hasScannedThisEnter) return;

        // If a specific scan volume collider is assigned, ensure THIS script is on that same object.
        // (Unity doesn't tell us which collider on this object fired the trigger.)
        if (!IsDogMatch(other)) return;

        DoScanAndUIUpdate();

        if (DialogueManager.Instance != null &&
            DialogueManager.Instance.IsAtStage(dialogueSegmentName, dialogueLineIndex))
        {
            DialogueManager.Instance.AdvanceDialogue();
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
        // Primary: exact collider match
        if (dogTargetCollider != null)
            return other == dogTargetCollider;

        // Fallback: layer + tag filters
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
            celsiusText.text = c.ToString($"F{decimalPlaces}") + celsiusSuffix;

        if (fahrenheitText != null)
            fahrenheitText.text = f.ToString($"F{decimalPlaces}") + fahrenheitSuffix;

        Debug.Log($"[IRThermalScannerTrigger] Scan: {celsiusText?.text} / {fahrenheitText?.text}");
    }
}
