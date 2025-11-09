using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class TextChunk
{
    [TextArea(3, 10)]
    public string text;
    public Vector2 position; // x, y offset from center
    public float fontSize = 0f; // 0 = use default
    public TextAlignmentOptions alignment = TextAlignmentOptions.Center;
}


public class ChalkSwipeWhiteboard : MonoBehaviour
{
    [Header("Swipe Sprites")]
    public Image[] swipeSprites;

    [Header("References")]
    
    [Header("Sprites Sequence")]
public Sprite[] spriteSequence; // The images you want to cycle through
private int currentSpriteIndex = 0; 
    public TextMeshProUGUI textMesh;
    public Transform rightHandTransform;

    [Header("TOUCH-Based Detection")]
    [Tooltip("REQUIRED: Hand must be this close to the board surface")]
    public float touchDistance = 0.1f; // 10cm - must almost touch the board

    [Tooltip("REQUIRED: Minimum lateral swipe distance while touching")]
    public float minSwipeDistance = 0.3f; // 30cm clear intentional swipe

    [Tooltip("Cooldown between swipes")]
    public float cooldown = 1.5f;

    [Header("Multi-line Pages")]
    public TextChunk[] pages;

    [Header("Chalk Animation")]
    public float eraseTime = 0.5f;
    public float charRevealDelay = 0.02f;

    [Header("Particles & Sound (Optional)")]
    public ParticleSystem chalkParticle;
    public AudioClip chalkWriteSfx;
    public AudioClip chalkEraseSfx;
    public AudioSource audioSource;

    [Header("Debug")]
    public bool showDebugGizmos = true;
    public bool logSwipeEvents = true;

    // Internal state
    private int currentIndex = 0;
    private Vector3 lastHandPos;
    private float lastTriggerTime = -10f;
    private bool isAnimating = false;

    // Swipe detection state
    private Vector3 swipeStartPos;
    private bool swipeInProgress = false;
    private float swipeStartTime;
    private bool wasTouching = false;

    void Start()
    {
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro reference is not assigned.", this);
            return;
        }

        if (rightHandTransform == null)
            TryAutoFindRightHand();

        // Initialize display
        if (pages != null && pages.Length > 0)
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, pages.Length - 1);

            // Force TMP to update mesh before animation
            DisplayPage(currentIndex);
            textMesh.ForceMeshUpdate();

            // Start the chalk animation for the first page
            StartCoroutine(PlayChalkTransition(currentIndex, skipErase: true));
        }

        lastHandPos = rightHandTransform ? rightHandTransform.position : Vector3.zero;
    }

    void Update()
    {
        if (rightHandTransform == null)
        {
            TryAutoFindRightHand();
            if (rightHandTransform == null) return;
        }

        // Quick cooldown check
        if (Time.time - lastTriggerTime < cooldown)
        {
            lastHandPos = rightHandTransform.position;
            return;
        }

        Vector3 handPos = rightHandTransform.position;
        Vector3 boardRight = transform.right;

        // Check if hand is touching the board
        bool isTouching = IsHandTouchingBoard(handPos);

        // Start swipe only when hand starts touching the board
        if (!swipeInProgress && isTouching && !wasTouching)
        {
            StartSwipe(handPos);
        }

        // Process ongoing swipe while touching
        if (swipeInProgress && isTouching)
        {
            ProcessSwipe(handPos, boardRight);
        }
        else if (swipeInProgress && !isTouching)
        {
            // Cancel if hand leaves board during swipe
            if (logSwipeEvents) Debug.Log("Swipe cancelled - hand left board");
            swipeInProgress = false;
        }

        wasTouching = isTouching;
        lastHandPos = handPos;
    }

    private void DisplayPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pages.Length) return;

        var chunk = pages[pageIndex];

        // Apply formatting
        textMesh.text = chunk.text;
        textMesh.alignment = chunk.alignment;

        if (chunk.fontSize > 0)
        {
            textMesh.fontSize = chunk.fontSize;
        }

        // Apply position offset
        RectTransform rectTransform = textMesh.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = chunk.position;
        }
    }

    private bool IsHandTouchingBoard(Vector3 handPos)
    {
        // Calculate distance to board plane
        Vector3 boardPosition = transform.position;
        Vector3 boardNormal = transform.forward; // Board faces forward

        // Distance from hand to board plane
        float distance = Mathf.Abs(Vector3.Dot(handPos - boardPosition, boardNormal));

        // Also check if hand is within the board bounds
        Vector3 localPos = transform.InverseTransformPoint(handPos);
        bool inBounds = Mathf.Abs(localPos.x) < 1.0f && Mathf.Abs(localPos.y) < 0.7f;

        return distance <= touchDistance && inBounds;
    }

    private void StartSwipe(Vector3 handPos)
    {
        swipeStartPos = handPos;
        swipeInProgress = true;
        swipeStartTime = Time.time;

        if (logSwipeEvents)
            Debug.Log($"Swipe STARTED - Hand touching board at: {handPos}");
    }

    private void ProcessSwipe(Vector3 currentHandPos, Vector3 boardRight)
    {
        Vector3 totalMovement = currentHandPos - swipeStartPos;
        float lateralDistance = Vector3.Dot(totalMovement, boardRight);

        // Check for swipe completion
        if (Mathf.Abs(lateralDistance) >= minSwipeDistance)
        {
            // Validate it's a reasonable swipe (not too fast/slow)
            float swipeDuration = Time.time - swipeStartTime;
            float swipeSpeed = totalMovement.magnitude / Mathf.Max(swipeDuration, 0.001f);

            if (swipeSpeed > 0.5f && swipeSpeed < 4.0f && swipeDuration < 2.0f)
            {
                // Valid swipe detected!
                if (lateralDistance > 0)
                    OnSwipeRight();
                else
                    OnSwipeLeft();

                lastTriggerTime = Time.time;

                if (logSwipeEvents)
                    Debug.Log($"VALID SWIPE: {(lateralDistance > 0 ? "RIGHT" : "LEFT")}, " +
                             $"Distance: {Mathf.Abs(lateralDistance):F3}m, Duration: {swipeDuration:F2}s");
            }
            else
            {
                if (logSwipeEvents)
                    Debug.Log($"Swipe rejected - Speed: {swipeSpeed:F2}m/s, Duration: {swipeDuration:F2}s");
            }

            swipeInProgress = false;
        }

        // Cancel swipe if it takes too long
        if (Time.time - swipeStartTime > 2.0f)
        {
            if (logSwipeEvents) Debug.Log("Swipe timeout");
            swipeInProgress = false;
        }
    }
private void OnSwipeRight()
{
    if (isAnimating) return;
    int newIndex = (currentIndex + 1) % pages.Length;
    if (newIndex != currentIndex)
    {
        currentIndex = newIndex;
        StartCoroutine(PlayChalkTransition(currentIndex));
        UpdateSwipeSprites();
    }
}

private void OnSwipeLeft()
{
    if (isAnimating) return;
    int newIndex = (currentIndex - 1 + pages.Length) % pages.Length;
    if (newIndex != currentIndex)
    {
        currentIndex = newIndex;
        StartCoroutine(PlayChalkTransition(currentIndex));
        UpdateSwipeSprites();
    }
}
private void UpdateSwipeSprites()
{
    if (swipeSprites == null || swipeSprites.Length == 0 || spriteSequence == null || spriteSequence.Length == 0)
        return;

    // Cycle sprite index
    currentSpriteIndex = (currentSpriteIndex + 1) % spriteSequence.Length;

    // Update all swipeSprites to the new sprite (or you can assign individually)
    foreach (var img in swipeSprites)
    {
        if (img != null)
            img.sprite = spriteSequence[currentSpriteIndex];
    }
}


    private IEnumerator PlayChalkTransition(int pageIndex, bool skipErase = false)
    {
        isAnimating = true;

        // Erase animation (skip for first page if desired)
        if (!skipErase)
        {
            if (chalkParticle != null) chalkParticle.Play();
            if (audioSource != null && chalkEraseSfx != null)
                audioSource.PlayOneShot(chalkEraseSfx);

            int totalChars = textMesh.textInfo.characterCount;
            float t = 0f;
            while (t < eraseTime)
            {
                t += Time.deltaTime;
                float progress = t / eraseTime;
                int charsToShow = totalChars - Mathf.RoundToInt(progress * totalChars);
                textMesh.maxVisibleCharacters = Mathf.Clamp(charsToShow, 0, totalChars);
                yield return null;
            }
            textMesh.maxVisibleCharacters = 0;

            yield return new WaitForSeconds(0.1f);
        }

        // Set new text and play write effects
        DisplayPage(pageIndex);
        textMesh.ForceMeshUpdate();

        if (chalkParticle != null) chalkParticle.Play();
        if (audioSource != null && chalkWriteSfx != null)
            audioSource.PlayOneShot(chalkWriteSfx);

        // Write animation
        int newTotal = textMesh.textInfo.characterCount;
        textMesh.maxVisibleCharacters = 0;

        for (int i = 0; i <= newTotal; i++)
        {
            textMesh.maxVisibleCharacters = i;
            yield return new WaitForSeconds(charRevealDelay);
        }

        isAnimating = false;
    }


    private void TryAutoFindRightHand()
    {
        string[] names = {
            "RightHand", "Right Hand", "right_hand",
            "RightHandAnchor", "RightHandAnchor(Clone)",
            "right_hand_controller", "RightController"
        };

        foreach (var name in names)
        {
            GameObject go = GameObject.Find(name);
            if (go != null)
            {
                rightHandTransform = go.transform;
                Debug.Log($"[ChalkSwipeWhiteboard] Auto-assigned right hand: {name}", this);
                return;
            }
        }

        Debug.LogWarning("[ChalkSwipeWhiteboard] Could not auto-find right hand transform.", this);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        // Draw touch zone
        Gizmos.color = swipeInProgress ? Color.green : Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;

        // Draw board surface
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(2f, 1.4f, 0.05f));

        // Draw touch distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(0, 0, -touchDistance), new Vector3(2f, 1.4f, 0.01f));

        Gizmos.matrix = Matrix4x4.identity;

        // Draw hand and swipe
        if (rightHandTransform != null)
        {
            bool touching = IsHandTouchingBoard(rightHandTransform.position);
            Gizmos.color = touching ? Color.green : Color.white;
            Gizmos.DrawSphere(rightHandTransform.position, 0.03f);

            if (swipeInProgress)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(swipeStartPos, rightHandTransform.position);
                Gizmos.DrawSphere(swipeStartPos, 0.02f);
            }
        }
    }

    public void GoToPage(int pageIndex)
    {
        if (isAnimating || pageIndex < 0 || pageIndex >= pages.Length) return;

        currentIndex = pageIndex;
        if (!isAnimating)
        {
            StartCoroutine(PlayChalkTransition(currentIndex));
        }
    }
}