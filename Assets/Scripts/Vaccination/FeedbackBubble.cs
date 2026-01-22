using System;
using UnityEngine;

public class FeedbackBubble : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private FeedbackType type = FeedbackType.Correct;
    public FeedbackType Type => type;

    [Header("Motion")]
    [SerializeField] private float lifetime = 1.2f;
    [SerializeField] private float riseSpeed = 0.35f;
    [SerializeField] private Vector3 localRiseDirection = Vector3.up;

    [Header("Fade")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool faceCamera = true;

    [Header("Scale Randomization")]
    [Tooltip("Minimum uniform scale for bubble.")]
    [SerializeField] private float minScale = 0.02f;

    [Tooltip("Maximum uniform scale for bubble.")]
    [SerializeField] private float maxScale = 0.06f;

    private float time;
    private Action<FeedbackBubble> returnToPool;
    private Vector3 baseScale;

    private void Awake()
    {
        // Store the prefab's original scale as reference
        baseScale = transform.localScale;
    }

    public void SetPoolReturn(Action<FeedbackBubble> returnFn)
    {
        returnToPool = returnFn;
    }

    public void Play()
    {
        time = 0f;

        // --- RANDOM SCALE ---
        float random = UnityEngine.Random.Range(minScale, maxScale);
        transform.localScale = baseScale * random;

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    private void Update()
    {
        time += Time.deltaTime;

        transform.position += transform.TransformDirection(localRiseDirection) * (riseSpeed * Time.deltaTime);

        if (faceCamera && Camera.main != null)
        {
            Vector3 dir = transform.position - Camera.main.transform.position;
            if (dir.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        }

        if (canvasGroup != null)
        {
            float t = Mathf.Clamp01(time / lifetime);
            canvasGroup.alpha = 1f - t;
        }

        if (time >= lifetime)
        {
            if (returnToPool != null)
                returnToPool(this);
            else
                gameObject.SetActive(false);
        }
    }
}
