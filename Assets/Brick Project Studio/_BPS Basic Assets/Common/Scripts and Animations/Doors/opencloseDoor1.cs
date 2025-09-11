using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRDoor : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool isOpen = false;

    // Called when you interact with the door (via XR Direct Interactor or Ray Interactor)
    public void ToggleDoor()
    {
        if (isOpen)
        {
            animator.Play("Closing 1");
            isOpen = false;
        }
        else
        {
            animator.Play("Opening 1");
            isOpen = true;
        }
    }
}
