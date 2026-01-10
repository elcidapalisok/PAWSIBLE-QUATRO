using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CageDoorController : MonoBehaviour
{
    private Animator animator;
    private bool isOpen;

    void Awake()
    {
        animator = GetComponent<Animator>();
        isOpen = false;
        animator.SetBool("Open", false);
    }
    //
    public void ToggleDoor()
    {
        isOpen = !isOpen;
        animator.SetBool("Open", isOpen);
        animator.SetTrigger("Toggle");
    }
}
