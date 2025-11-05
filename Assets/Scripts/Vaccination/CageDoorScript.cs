using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DoorScript : XRBaseInteractable
{
    private Animator animator;
    private bool isOpen = false;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }
}
