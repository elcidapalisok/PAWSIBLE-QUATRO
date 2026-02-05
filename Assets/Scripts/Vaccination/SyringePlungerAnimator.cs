using UnityEngine;

public class SyringePlungerAnimator : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Trigger Names")]
    [SerializeField] private string pullTrigger = "DoPull";
    [SerializeField] private string pushTrigger = "DoPush";

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError(name + ": SyringePlungerAnimator has no Animator reference.");
    }

    public void PlayPull()
    {
        if (animator == null) return;
        animator.ResetTrigger(pushTrigger);
        animator.SetTrigger(pullTrigger);
    }

    public void PlayPush()
    {
        if (animator == null) return;
        animator.ResetTrigger(pullTrigger);
        animator.SetTrigger(pushTrigger);
    }
}
