using UnityEngine;

public class SyringePlungerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("State Names")]
    [SerializeField] private string fillStateName = "fill";
    [SerializeField] private string emptyStateName = "empty";

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void PlayFill()
    {
        if (animator == null) return;
        animator.Play(fillStateName, 0, 0f);
    }

    public void PlayEmpty()
    {
        if (animator == null) return;
        animator.Play(emptyStateName, 0, 0f);
    }
}
