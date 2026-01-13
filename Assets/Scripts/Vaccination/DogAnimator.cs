using UnityEngine;

public class DogAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private const string INTERACT = "IsInteracting";
    private const string POSE = "Pose";

    private const int IDLE = 0;
    private const int LAY = 1;
    private const int SIT = 2;
    private const int PAW = 3;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void BeginInteract()
    {
        animator.SetBool(INTERACT, true);
    }

    public void EndInteract()
    {
        animator.SetBool(INTERACT, false);
        animator.SetInteger(POSE, IDLE);
    }

    public void Lay()
    {
        animator.SetBool(INTERACT, true);
        animator.SetInteger(POSE, LAY);
    }

    public void Sit()
    {
        animator.SetBool(INTERACT, true);
        animator.SetInteger(POSE, SIT);
    }

    public void Paw()
    {
        animator.SetBool(INTERACT, true);
        animator.SetInteger(POSE, PAW);
    }
}
