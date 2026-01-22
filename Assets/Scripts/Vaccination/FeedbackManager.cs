using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FeedbackType
{
    Correct,
    Wrong
}

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }

    [Header("Visual Prefabs")]
    [SerializeField] private FeedbackBubble correctBubblePrefab;
    [SerializeField] private FeedbackBubble wrongBubblePrefab;

    [Header("Pool Settings")]
    [SerializeField] private int preloadPerType = 8;

    [Header("Burst Settings")]
    [Min(1)] public int correctBurstCount = 4;
    [Min(1)] public int wrongBurstCount = 3;

    [Tooltip("Random offset around spawn position (meters).")]
    public Vector3 spawnJitter = new Vector3(0.08f, 0.02f, 0.08f);

    [Tooltip("Delay between bubbles in the burst (seconds). 0 = instant.")]
    [Min(0f)] public float burstStagger = 0.05f;

    [Header("Audio")]
    [SerializeField] private AudioSource feedbackAudioSource;
    [SerializeField] private AudioClip correctSfx;
    [SerializeField] private AudioClip wrongSfx;

    private readonly Queue<FeedbackBubble> correctPool = new Queue<FeedbackBubble>();
    private readonly Queue<FeedbackBubble> wrongPool = new Queue<FeedbackBubble>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple FeedbackManager instances found. Keeping the first instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Preload();
    }

    private void Preload()
    {
        if (correctBubblePrefab != null)
        {
            for (int i = 0; i < preloadPerType; i++)
                correctPool.Enqueue(CreateBubble(correctBubblePrefab));
        }

        if (wrongBubblePrefab != null)
        {
            for (int i = 0; i < preloadPerType; i++)
                wrongPool.Enqueue(CreateBubble(wrongBubblePrefab));
        }
    }

    private FeedbackBubble CreateBubble(FeedbackBubble prefab)
    {
        FeedbackBubble bubble = Instantiate(prefab, transform);
        bubble.gameObject.SetActive(false);
        bubble.SetPoolReturn(ReturnToPool);
        return bubble;
    }

    public void ReportCorrect(Vector3 worldPosition)
    {
        PlaySfx(FeedbackType.Correct);
        StartCoroutine(SpawnBurst(FeedbackType.Correct, worldPosition, correctBurstCount));
    }

    public void ReportWrong(Vector3 worldPosition)
    {
        PlaySfx(FeedbackType.Wrong);
        StartCoroutine(SpawnBurst(FeedbackType.Wrong, worldPosition, wrongBurstCount));
    }

    private IEnumerator SpawnBurst(FeedbackType type, Vector3 origin, int count)
    {
        for (int i = 0; i < count; i++)
        {
            FeedbackBubble bubble = GetFromPool(type);
            if (bubble != null)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-spawnJitter.x, spawnJitter.x),
                    Random.Range(0f, spawnJitter.y),
                    Random.Range(-spawnJitter.z, spawnJitter.z)
                );

                bubble.transform.position = origin + offset;
                bubble.transform.rotation = Quaternion.identity;
                bubble.gameObject.SetActive(true);
                bubble.Play();
            }

            if (burstStagger > 0f && i < count - 1)
                yield return new WaitForSeconds(burstStagger);
        }
    }

    private FeedbackBubble GetFromPool(FeedbackType type)
    {
        if (type == FeedbackType.Correct)
        {
            if (correctBubblePrefab == null) return null;
            if (correctPool.Count > 0) return correctPool.Dequeue();
            return CreateBubble(correctBubblePrefab);
        }

        if (wrongBubblePrefab == null) return null;
        if (wrongPool.Count > 0) return wrongPool.Dequeue();
        return CreateBubble(wrongBubblePrefab);
    }

    private void ReturnToPool(FeedbackBubble bubble)
    {
        if (bubble == null) return;

        bubble.gameObject.SetActive(false);

        if (bubble.Type == FeedbackType.Correct)
            correctPool.Enqueue(bubble);
        else
            wrongPool.Enqueue(bubble);
    }

    private void PlaySfx(FeedbackType type)
    {
        if (feedbackAudioSource == null) return;

        AudioClip clip = (type == FeedbackType.Correct) ? correctSfx : wrongSfx;
        if (clip == null) return;

        feedbackAudioSource.PlayOneShot(clip);
    }
}
