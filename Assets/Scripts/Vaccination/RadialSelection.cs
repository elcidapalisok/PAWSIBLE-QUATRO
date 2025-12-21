using System.Collections.Generic;
using UnityEngine;

public class RadialSelection : MonoBehaviour
{
    [Header("Radial Setup")]
    [SerializeField] private RadialPart radialPartPrefab;
    [SerializeField] private RectTransform radialCanvas;
    [SerializeField] private int numberOfParts = 4;

    [Tooltip("Gap between slices (0.02–0.08 recommended)")]
    [SerializeField] private float sliceSpacing = 0.05f;

    [Header("XR Behaviour")]
    [SerializeField] private Transform xrHead;
    [SerializeField] private float heightOffset = 0.4f;
    [SerializeField] private float closeDistance = 3f;

    private readonly List<RadialPart> parts = new();
    private Transform targetDog;
    private bool menuOpen;

    void Start()
    {
        if (!xrHead)
            xrHead = Camera.main.transform; // fallback

        CreateRadialParts();
        CloseMenu();
    }

    void Update()
    {
        if (!menuOpen || targetDog == null)
            return;

        UpdatePosition();
        AutoCloseCheck();
    }

    public void OpenMenu(Transform dog)
    {
        targetDog = dog;
        menuOpen = true;
        radialCanvas.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        menuOpen = false;
        radialCanvas.gameObject.SetActive(false);
        targetDog = null;
    }

    void UpdatePosition()
    {
        transform.position = targetDog.position + Vector3.up * heightOffset;
        transform.LookAt(xrHead);
        transform.Rotate(0f, 180f, 0f);
    }

    void AutoCloseCheck()
    {
        if (Vector3.Distance(xrHead.position, targetDog.position) > closeDistance)
        {
            CloseMenu();
        }
    }

    void CreateRadialParts()
    {
        parts.Clear();

        float baseFill = 1f / numberOfParts;
        float finalFill = Mathf.Clamp(baseFill - sliceSpacing, 0.01f, 1f);
        float angleStep = 360f / numberOfParts;

        for (int i = 0; i < numberOfParts; i++)
        {
            RadialPart part = Instantiate(radialPartPrefab, radialCanvas);
            part.Initialize(i);
            part.SetFill(finalFill);
            part.SetRotation(-angleStep * i);
            parts.Add(part);
        }
    }
}
