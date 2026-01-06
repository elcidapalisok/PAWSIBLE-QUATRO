using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class BreakableVial : MonoBehaviour
{
    [Header("Vial Objects")]
    public GameObject wholeVial;
    public GameObject brokenVial;

    [Header("Break Settings")]
    [Tooltip("Relative velocity required to break")]
    public float breakForceThreshold = 2.0f;

    [Tooltip("Grace time after XR release before breaking allowed")]
    public float releaseGraceTime = 0.05f;

    [Header("Shard Settings")]
    public float shardImpulse = 0.5f;

    private bool isBroken = false;
    private bool canBreak = true;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (brokenVial != null)
            brokenVial.SetActive(false);

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        canBreak = false;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        canBreak = false;
        Invoke(nameof(EnableBreaking), releaseGraceTime);
    }

    private void EnableBreaking()
    {
        canBreak = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isBroken || !canBreak)
            return;

        float impactStrength = collision.relativeVelocity.magnitude;

        Debug.Log($"[{name}] Impact velocity: {impactStrength}");

        if (impactStrength >= breakForceThreshold)
        {
            BreakVial();
        }
    }

    void BreakVial()
    {
        if (isBroken)
            return;

        isBroken = true;

        Debug.Log($"[{name}] BreakVial() executed");

        // Force release if still grabbed
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            grabInteractable.interactionManager.SelectExit(
                grabInteractable.firstInteractorSelecting,
                grabInteractable
            );
        }

        if (grabInteractable != null)
            grabInteractable.enabled = false;

        // ---- CRITICAL FIXES START HERE ----

        // Detach broken vial to avoid compound Rigidbody issues
        brokenVial.transform.SetParent(null, true);

        // Disable intact model
        if (wholeVial != null)
            wholeVial.SetActive(false);

        // Enable broken model
        if (brokenVial != null)
            brokenVial.SetActive(true);

        // Force renderers ON (imported mesh safety)
        foreach (Renderer r in brokenVial.GetComponentsInChildren<Renderer>(true))
        {
            r.enabled = true;
        }

        // Activate shard physics safely
        foreach (Rigidbody shardRb in brokenVial.GetComponentsInChildren<Rigidbody>())
        {
            shardRb.isKinematic = false;
            shardRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            shardRb.WakeUp();
            shardRb.AddForce(Random.insideUnitSphere * shardImpulse, ForceMode.Impulse);
        }

        // Optional cleanup
        Destroy(gameObject, 5f);
    }
}
