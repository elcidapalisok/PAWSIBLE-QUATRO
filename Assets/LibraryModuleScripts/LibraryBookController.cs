using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LibraryBookController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image organImage;
    [SerializeField] private TMP_Text organNameText;
    [SerializeField] private TMP_Text organDescriptionText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.2f;


    [Header("Data")]
    [SerializeField] private List<OrganEntry> organs = new List<OrganEntry>();

    private int currentIndex = 0;

    private void Awake()
    {
        // Hook up button events
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (prevButton != null) prevButton.onClick.AddListener(PrevPage);
    }

    private void Start()
    {
        ShowPage(0);
    }

    private void ShowPage(int index)
    {
        if (organs == null || organs.Count == 0)
        {
            // Safe fallback if nothing is set yet
            if (organNameText) organNameText.text = "No organs added";
            if (organDescriptionText) organDescriptionText.text = "Add entries in the Inspector.";
            if (organImage) organImage.sprite = null;
            UpdateButtons();
            return;
        }

        currentIndex = Mathf.Clamp(index, 0, organs.Count - 1);
        OrganEntry entry = organs[currentIndex];

        if (organNameText) organNameText.text = entry.organName;
        if (organDescriptionText) organDescriptionText.text = entry.description;

        if (organImage)
        {
            organImage.sprite = entry.organSprite;
            organImage.preserveAspect = true;
        }
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();


        UpdateButtons();
    }

    private void UpdateButtons()
    {
        if (prevButton) prevButton.interactable = currentIndex > 0;
        if (nextButton) nextButton.interactable = currentIndex < organs.Count - 1;
    }

    public void NextPage()
    {
        StartCoroutine(ChangePageWithFade(currentIndex + 1));
    }


    public void PrevPage()
    {
        StartCoroutine(ChangePageWithFade(currentIndex - 1));
    }
    private IEnumerator ChangePageWithFade(int newIndex)
    {
        // Fade out
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;

        // Change content
        ShowPage(newIndex);

        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

}
