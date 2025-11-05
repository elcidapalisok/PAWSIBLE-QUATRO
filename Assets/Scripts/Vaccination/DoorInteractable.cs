using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class DoorInteractable : MonoBehaviour
{
    private Animator animator;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
    private bool isOpen = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        // Listen for grab (select) events from XR Interaction Toolkit
        interactable.selectEntered.AddListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Toggle the door state
        isOpen = !isOpen;

        // Update Animator parameter (make sure your Animator has a Bool named "IsOpen")
        animator.SetBool("IsOpen", isOpen);

        Debug.Log($"Door state toggled. IsOpen = {isOpen}");
    }

    private void OnDestroy()
    {
        // Always unsubscribe from events to avoid memory leaks
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnGrab);
    }
    void Start()
    {
        if (animator.runtimeAnimatorController == null)
            Debug.LogError("Animator Controller is missing on the door!");
        else
            Debug.Log("Animator Controller found: " + animator.runtimeAnimatorController.name);
    }

}
