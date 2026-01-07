using UnityEngine;

public class DogAnimator : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Sit()
    {
        animator.SetBool("IsIdle", false);
        ResetTriggers();
        animator.SetTrigger("Sit");
    }

    public void Lay()
    {
        animator.SetBool("IsIdle", false);
        ResetTriggers();
        animator.SetTrigger("Lay");
    }

    public void Paw()
    {
        animator.SetBool("IsIdle", false);
        ResetTriggers();
        animator.SetTrigger("Paw");
    }

    public void ReturnToIdle()
    {
        animator.SetBool("IsIdle", true);
    }

    private void ResetTriggers()
    {
        animator.ResetTrigger("Sit");
        animator.ResetTrigger("Lay");
        animator.ResetTrigger("Paw");
    }
}
