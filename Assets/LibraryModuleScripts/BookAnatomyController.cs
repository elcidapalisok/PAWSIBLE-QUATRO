using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BookAnatomyController : MonoBehaviour
{
    [Header("Models")]
    [SerializeField] private GameObject anatomyBookClosed;
    [SerializeField] private GameObject anatomyBookOpenFlat;

    [Header("Animators")]
    [SerializeField] private Animator closedAnimator;
    [SerializeField] private Animator openFlatAnimator;

    [Header("Closed Book Triggers")]
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string closeTrigger = "Close";

    [Header("Open-Flat Book Triggers")]
    [SerializeField] private string nextTrigger = "Next";
    [SerializeField] private string prevTrigger = "Prev";

    [Header("Timing")]
    [SerializeField] private float swapDelay = 0.50f;

    [Header("XR + Snap")]
    [SerializeField] private XRGrabInteractable grab;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform snapPoint;
    [SerializeField] private float snapDistance = 0.35f;

    [Header("Close Behavior")]
    [SerializeField] private bool playCloseWhenGrabbed = true;

    [Header("Debug")]
    [SerializeField] private bool logActions = true;

    // State
    private bool isOnPodium = false;
    private bool swappedToOpenFlat = false;

    private Coroutine swapRoutine;
    private Coroutine delayedRemoveRoutine;
    private float lastPodiumContactTime = -999f;

    private bool isHeldByHand = false; // Only true for hand/controller interactor (NOT socket)

    public bool CanTurnPage => isOnPodium && swappedToOpenFlat;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
    }

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!grab) grab = GetComponent<XRGrabInteractable>();

        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (grab)
        {
            grab.selectEntered.AddListener(OnSelectEntered);
            grab.selectExited.AddListener(OnSelectExited);
        }
    }

    private void Start()
    {
        ForceClosedOnly(playCloseAnim: false);
    }

    private void OnDestroy()
    {
        if (grab)
        {
            grab.selectEntered.RemoveListener(OnSelectEntered);
            grab.selectExited.RemoveListener(OnSelectExited);
        }
    }

    // ===================== OPTION B CORE =====================
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Selected by socket => treat as placed, NOT grabbed
        if (args.interactorObject is XRSocketInteractor)
        {
            if (logActions) Debug.Log("[Book] Selected by SOCKET (not treated as grabbed)");
            return;
        }

        // Otherwise treat as hand/controller grab
        isHeldByHand = true;
        if (logActions) Debug.Log("[Book] Selected by HAND/CONTROLLER -> forcing closed");
        HandleGrabbed();
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Deselected by socket => ignore for hand release logic
        if (args.interactorObject is XRSocketInteractor)
        {
            if (logActions) Debug.Log("[Book] Deselected by SOCKET");
            return;
        }

        isHeldByHand = false;
        if (logActions) Debug.Log("[Book] Released by HAND/CONTROLLER");
        HandleReleased();
    }
    // =========================================================

    // Called by podium zone (trigger enter/stay)
    public void PlacedOnPodium(Transform podiumSnapPoint)
    {
        snapPoint = podiumSnapPoint;
        lastPodiumContactTime = Time.time;

        // Cancel pending delayed removal
        if (delayedRemoveRoutine != null)
        {
            StopCoroutine(delayedRemoveRoutine);
            delayedRemoveRoutine = null;
        }

        isOnPodium = true;

        // Do NOT open while held by hand/controller
        if (isHeldByHand) return;

        SnapToPoint();

        if (swappedToOpenFlat) return;
        if (swapRoutine != null) return;

        if (logActions) Debug.Log("[Book] Trigger Open");
        swapRoutine = StartCoroutine(OpenThenSwap());
    }

    // Called by podium zone exit; use grace seconds
    public void RemovedFromPodium(float graceSeconds)
    {
        if (delayedRemoveRoutine != null)
        {
            StopCoroutine(delayedRemoveRoutine);
            delayedRemoveRoutine = null;
        }

        delayedRemoveRoutine = StartCoroutine(DelayedRemove(graceSeconds));
    }

    private IEnumerator DelayedRemove(float graceSeconds)
    {
        yield return new WaitForSeconds(graceSeconds);

        // If podium contact happened recently, ignore
        if (Time.time - lastPodiumContactTime <= graceSeconds)
        {
            delayedRemoveRoutine = null;
            yield break;
        }

        // If held by hand, ignore
        if (isHeldByHand)
        {
            delayedRemoveRoutine = null;
            yield break;
        }

        isOnPodium = false;
        ForceClosedOnly(playCloseAnim: false);
        delayedRemoveRoutine = null;
    }

    private IEnumerator OpenThenSwap()
    {
        if (anatomyBookClosed) anatomyBookClosed.SetActive(true);
        if (anatomyBookOpenFlat) anatomyBookOpenFlat.SetActive(false);
        swappedToOpenFlat = false;

        if (closedAnimator)
        {
            closedAnimator.ResetTrigger(closeTrigger);
            closedAnimator.SetTrigger(openTrigger);
        }

        yield return new WaitForSeconds(swapDelay);

        if (isHeldByHand)
        {
            swapRoutine = null;
            yield break;
        }

        if (!isOnPodium)
        {
            swapRoutine = null;
            yield break;
        }

        if (anatomyBookClosed) anatomyBookClosed.SetActive(false);
        if (anatomyBookOpenFlat) anatomyBookOpenFlat.SetActive(true);
        swappedToOpenFlat = true;

        swapRoutine = null;
    }

    private void HandleGrabbed()
    {
        isOnPodium = false;
        ForceClosedOnly(playCloseAnim: playCloseWhenGrabbed);
    }

    private void HandleReleased()
    {
        // If released near podium, snap and open again
        if (snapPoint != null && Vector3.Distance(transform.position, snapPoint.position) <= snapDistance)
        {
            isOnPodium = true;
            lastPodiumContactTime = Time.time;

            SnapToPoint();

            if (!swappedToOpenFlat && swapRoutine == null && !isHeldByHand)
                swapRoutine = StartCoroutine(OpenThenSwap());
        }
        else
        {
            isOnPodium = false;
            ForceClosedOnly(playCloseAnim: false);
        }
    }

    private void ForceClosedOnly(bool playCloseAnim)
    {
        if (swapRoutine != null)
        {
            StopCoroutine(swapRoutine);
            swapRoutine = null;
        }
        if (delayedRemoveRoutine != null)
        {
            StopCoroutine(delayedRemoveRoutine);
            delayedRemoveRoutine = null;
        }

        if (anatomyBookClosed) anatomyBookClosed.SetActive(true);
        if (anatomyBookOpenFlat) anatomyBookOpenFlat.SetActive(false);
        swappedToOpenFlat = false;

        if (playCloseAnim && closedAnimator)
        {
            closedAnimator.ResetTrigger(openTrigger);
            closedAnimator.SetTrigger(closeTrigger);
        }
    }

    private void SnapToPoint()
    {
        if (!snapPoint) return;

        transform.SetPositionAndRotation(snapPoint.position, snapPoint.rotation);

        if (rb)
        {
            // Keep stable; avoid velocity writes on kinematic
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    // UI hooks
    public void NextPage()
    {
        if (!CanTurnPage) return;
        if (openFlatAnimator) openFlatAnimator.SetTrigger(nextTrigger);
    }

    public void PrevPage()
    {
        if (!CanTurnPage) return;
        if (openFlatAnimator) openFlatAnimator.SetTrigger(prevTrigger);
    }

    // ================= DEBUG (READ-ONLY) =================
    public bool Debug_IsOnPodium => isOnPodium;
    public bool Debug_IsGrabbed => isHeldByHand; // IMPORTANT: true only for hand/controller
    public bool Debug_Swapped => swappedToOpenFlat;
    public bool Debug_OpenFlatActive => anatomyBookOpenFlat != null && anatomyBookOpenFlat.activeSelf;
    public bool Debug_SwapRoutineRunning => swapRoutine != null;
}
