using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class HazardBinController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float destroyDelay = 0.2f;

    [Header("Contents")]
    [SerializeField] private Collider contentsTrigger;

    private Animator animator;
    private XRSimpleInteractable interactable;

    private readonly List<GameObject> contents = new List<GameObject>();

    void Awake()
    {
        animator = GetComponent<Animator>();
        interactable = GetComponent<XRSimpleInteractable>();

        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
    }

    // -------------------------
    // XR HOVER EVENTS
    // -------------------------

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        animator.SetBool("Open", true);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        animator.SetBool("Open", false);
        Invoke(nameof(DestroyContents), destroyDelay);
    }

    // -------------------------
    // CONTENT HANDLING
    // -------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (!contents.Contains(other.gameObject))
        {
            contents.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        contents.Remove(other.gameObject);
    }

    private void DestroyContents()
    {
        foreach (GameObject obj in contents)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        contents.Clear();
    }

    // -------------------------
    // SAFETY
    // -------------------------

    void OnDestroy()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
    }
}
