using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BreakableVial : MonoBehaviour
{
    [Header("Vial Objects")]
    public GameObject wholeVial;
    public GameObject brokenVial;

    [Header("Break Settings")]
    public float breakForceThreshold = 0.7f;

    private bool isBroken = false;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isBroken)
            return;

        Debug.Log("Collision with: " + collision.gameObject.name);

        // Do not break while being held
        if (grabInteractable != null && grabInteractable.isSelected)
            return;

        float impactImpulse = collision.impulse.magnitude;
        Debug.Log("Impact impulse: " + impactImpulse);

        if (impactImpulse >= breakForceThreshold)
        {
            BreakVial();
        }
    }

    void BreakVial()
    {
        isBroken = true;

        // Force release if being held
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            grabInteractable.interactionManager.SelectExit(
                grabInteractable.firstInteractorSelecting,
                grabInteractable
            );
        }

        // Disable grab
        if (grabInteractable != null)
            grabInteractable.enabled = false;

        // Disable intact model
        if (wholeVial != null)
            wholeVial.SetActive(false);

        // Enable fractured model
        if (brokenVial != null)
            brokenVial.SetActive(true);

        // Add impulse to shards
        foreach (Rigidbody shardRb in brokenVial.GetComponentsInChildren<Rigidbody>())
        {
            shardRb.AddForce(Random.insideUnitSphere * 0.5f, ForceMode.Impulse);
        }

        Destroy(gameObject, 5f);
    }
}
