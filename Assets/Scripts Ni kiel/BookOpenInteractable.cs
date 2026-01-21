using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class BookOpenInteractable : MonoBehaviour, IPointerClickHandler
{
    [Header("Animator")]
    public Animator animator;
    public string isOpenParam = "isOpen";

    [Header("Pause Settings")]
    [Range(0f, 1f)]
    public float pauseNormalizedTime = 0.15f; // 15% of animation

    [Header("UI (Canvas Group)")]
    public GameObject uiRoot;          // Canvas GameObject
    public CanvasGroup canvasGroup;    // CanvasGroup component
    public float fadeDuration = 0.25f;

    [Header("Cinemachine")]
    public CinemachineVirtualCamera virtualCamera;
    public int cameraPriorityOnOpen = 100;
    public int cameraPriorityOff = 0;

    private bool isOpen = false;
    private bool busy = false;

    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // Ensure UI starts hidden
        if (uiRoot != null)
            uiRoot.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        // Ensure camera starts low priority
        if (virtualCamera != null)
            virtualCamera.Priority = cameraPriorityOff;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (busy) return;

        if (!isOpen)
            StartCoroutine(OpenFlow());
        else
            StartCoroutine(CloseFlow());
    }

    IEnumerator OpenFlow()
    {
        busy = true;
        isOpen = true;

        // 1) Raise camera priority
        if (virtualCamera != null)
            virtualCamera.Priority = cameraPriorityOnOpen;

        // 2) Start open animation
        animator.speed = 1f;
        animator.SetBool(isOpenParam, true);

        // 3) Pause animation at 15%
        yield return PauseAnimatorAtNormalizedTime(pauseNormalizedTime);

        // 4) Show canvas + fade in
        if (uiRoot != null)
            uiRoot.SetActive(true);

        if (canvasGroup != null)
            yield return FadeCanvas(0f, 1f);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        busy = false;
    }

    IEnumerator CloseFlow()
    {
        busy = true;
        isOpen = false;

        // 1) Fade out UI first
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            yield return FadeCanvas(1f, 0f);
        }

        // 2) Hide canvas
        if (uiRoot != null)
            uiRoot.SetActive(false);

        // 3) Resume animation (close)
        animator.speed = 1f;
        animator.SetBool(isOpenParam, false);

        // 4) Lower camera priority
        if (virtualCamera != null)
            virtualCamera.Priority = cameraPriorityOff;

        busy = false;
    }

    IEnumerator PauseAnimatorAtNormalizedTime(float targetNormalizedTime)
    {
        // Wait until animation reaches the desired normalized time
        while (true)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            float currentNormalized = state.normalizedTime % 1f;

            if (currentNormalized >= targetNormalizedTime)
                break;

            yield return null;
        }

        animator.speed = 0f; // Pause animation
    }

    IEnumerator FadeCanvas(float from, float to)
    {
        if (canvasGroup == null) yield break;

        float time = 0f;
        canvasGroup.alpha = from;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}
