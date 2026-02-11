using UnityEngine;

public class ContainerFill : MonoBehaviour
{
    [Header("Water Visual (child object inside the container)")]
    public Transform waterVisual;

    [Header("Fill Settings")]
    [Range(0f, 1f)] public float fill = 0f;
    public float fillSpeed = 0.25f; // how fast it fills per second

    [Header("Water height range (LOCAL Y of WaterVisual)")]
    public float minLocalY = -0.02f;
    public float maxLocalY = 0.02f;

    [HideInInspector] public bool inFillZone = false;
    [HideInInspector] public bool faucetOn = false;

    void Start()
    {
        ApplyVisual();
    }

    void Update()
    {
        // Fill only when in zone + faucet on
        if (inFillZone && faucetOn)
        {
            fill = Mathf.Clamp01(fill + fillSpeed * Time.deltaTime);
            ApplyVisual();
        }
    }

    public void SetInFillZone(bool value)
    {
        inFillZone = value;

        // If you leave the zone and you're empty, hide it
        if (!inFillZone && fill <= 0.001f && waterVisual != null)
            waterVisual.gameObject.SetActive(false);
    }

    public void SetFaucetOn(bool value)
    {
        faucetOn = value;
    }

    void ApplyVisual()
    {
        if (waterVisual == null) return;

        // Hide when empty
        if (fill <= 0.001f)
        {
            waterVisual.gameObject.SetActive(false);
            return;
        }

        // Show when has water
        if (!waterVisual.gameObject.activeSelf)
            waterVisual.gameObject.SetActive(true);

        // Move water surface up/down (local Y)
        Vector3 p = waterVisual.localPosition;
        p.y = Mathf.Lerp(minLocalY, maxLocalY, fill);
        waterVisual.localPosition = p;
    }

    // Optional: call this if you want a reset button later
    public void Empty()
    {
        fill = 0f;
        ApplyVisual();
    }
}
