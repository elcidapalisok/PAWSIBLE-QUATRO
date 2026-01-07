using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RadialSelection : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RadialPartXR slicePrefab;

    [Header("Dog")]
    [SerializeField] private DogAnimator dogAnimator;

    [Header("Positioning")]
    [SerializeField] private float heightOffset = 1.2f;
    [SerializeField] private float closeDistance = 3f;

    private readonly List<RadialPartXR> slices = new();
    private Transform dog;
    private Transform xrCamera;
    private bool isOpen;

    void Awake()
    {
        xrCamera = Camera.main.transform;
        CreateSlices();
        canvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isOpen || dog == null) return;

        UpdatePosition();
        CheckDistance();
    }

    // ---------------- OPEN / CLOSE ----------------

    public void OpenMenu(Transform dogTransform)
    {
        dog = dogTransform;
        isOpen = true;
        canvas.gameObject.SetActive(true);
        UpdatePosition();
    }

    public void CloseMenu()
    {
        isOpen = false;
        canvas.gameObject.SetActive(false);
        dog = null;
    }

    // ---------------- POSITION ----------------

    void UpdatePosition()
    {
        transform.position = dog.position + Vector3.up * heightOffset;
        transform.LookAt(xrCamera);
        transform.Rotate(0f, 180f, 0f);
    }

    void CheckDistance()
    {
        if (Vector3.Distance(xrCamera.position, dog.position) > closeDistance)
        {
            dogAnimator.ReturnToIdle();
            CloseMenu();
        }
    }

    // ---------------- SLICES ----------------

    void CreateSlices()
    {
        string[] labels = { "Sit", "Lay", "Paw", "" };
        float fillAmount = 1f / labels.Length;
        float angleStep = 360f / labels.Length;

        for (int i = 0; i < labels.Length; i++)
        {
            RadialPartXR slice = Instantiate(slicePrefab, canvas);
            slice.Setup(i, labels[i]);

            Image img = slice.GetComponent<Image>();
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Radial360;
            img.fillAmount = fillAmount;

            // 🔥 THIS IS THE FIX
            img.rectTransform.localRotation =
                Quaternion.Euler(0, 0, -angleStep * i);

            slices.Add(slice);
        }
    }


    // Called by RadialPartXR
    public void OnSliceSelected(int index)
    {
        switch (index)
        {
            case 0:
                dogAnimator.Sit();
                break;
            case 1:
                dogAnimator.Lay();
                break;
            case 2:
                dogAnimator.Paw();
                break;
            case 3:
                // empty
                break;
        }

        CloseMenu();
    }
}
