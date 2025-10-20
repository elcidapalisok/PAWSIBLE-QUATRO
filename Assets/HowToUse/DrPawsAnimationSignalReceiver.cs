using UnityEngine;

public class DrPawsAnimationSignalReceiver : MonoBehaviour
{
    public Animator animator;

    // These public functions can be called from Timeline signals
    public void PlayIdle()
    {
        animator.SetTrigger("Idle");
        Debug.Log("Dr. Paws: Idle animation triggered.");
    }

    public void PlayWalk()
    {
        animator.SetTrigger("Walk");
        Debug.Log("Dr. Paws: Walk animation triggered.");
    }

    public void PlayGrab()
    {
        animator.SetTrigger("");
        Debug.Log("Dr. Paws: Grab animation triggered.");
    }
        public void PlayPut()
    {
        animator.SetTrigger("Grab");
        Debug.Log("Dr. Paws: Grab animation triggered.");
    }
}
