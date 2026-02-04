using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisceralLibraryController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image organImage;
    [SerializeField] private TMP_Text organNameText;
    [SerializeField] private TMP_Text organDescriptionText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [Header("Canvas Fading (Page Content)")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.2f;

    [Header("Book Animation Sync")]
    [Tooltip("Drag the BookAnatomyController from the BookAnatomy root here.")]
    [SerializeField] private BookAnatomyController bookController;

    [Header("Timing (Optional)")]
    [Tooltip("Small delay so the page-flip animation starts before the UI swaps content.")]
    [SerializeField] private float uiSwapDelayAfterFlipTrigger = 0.05f;

    [Header("Data")]
    [SerializeField] private List<OrganEntry> organs = new List<OrganEntry>();

    private int currentIndex = 0;
    private bool isChangingPage = false;

    private void Awake()
    {
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (prevButton != null) prevButton.onClick.AddListener(PrevPage);
    }

    private void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        ShowPage(0);
    }

    private void ShowPage(int index)
    {
        if (organs == null || organs.Count == 0)
        {
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

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        bool allowInput = !isChangingPage;

        if (prevButton)
            prevButton.interactable = allowInput && currentIndex > 0;

        if (nextButton)
            nextButton.interactable = allowInput && currentIndex < organs.Count - 1;
    }

    public void NextPage()
    {
        if (isChangingPage) return;
        if (organs == null || organs.Count == 0) return;
        if (currentIndex >= organs.Count - 1) return;

        if (bookController != null && !bookController.CanTurnPage) return;

        StartCoroutine(ChangePageWithFade(currentIndex + 1, isNext: true));
    }

    public void PrevPage()
    {
        if (isChangingPage) return;
        if (organs == null || organs.Count == 0) return;
        if (currentIndex <= 0) return;

        if (bookController != null && !bookController.CanTurnPage) return;

        StartCoroutine(ChangePageWithFade(currentIndex - 1, isNext: false));
    }

    private IEnumerator ChangePageWithFade(int newIndex, bool isNext)
    {
        isChangingPage = true;
        UpdateButtons();

        if (bookController != null)
        {
            if (isNext) bookController.NextPage();
            else bookController.PrevPage();
        }

        if (uiSwapDelayAfterFlipTrigger > 0f)
            yield return new WaitForSeconds(uiSwapDelayAfterFlipTrigger);

        float t = 0f;
        float startAlpha = (canvasGroup != null) ? canvasGroup.alpha : 1f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        ShowPage(newIndex);

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        isChangingPage = false;
        UpdateButtons();
    }

    public void GoToFirstPage()
    {
        if (isChangingPage) return;
        ShowPage(0);
    }
}
