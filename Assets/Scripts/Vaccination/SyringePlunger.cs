using UnityEngine;

public class SyringePlunger : MonoBehaviour
{
    public Animator animator;
    private bool isInjected = false;

    public void TriggerInjection()
    {
        if (!isInjected)
        {
            animator.Play("PlungerMove");
            isInjected = true;
        }
    }
}
