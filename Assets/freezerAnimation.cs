using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class freezerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool isOpen = false;

    public void ToggleDoor()
    {
        if (isOpen)
        {

            animator.Play("Armature|CloseFreezer"); // start at end
            isOpen = false;
        }
        else
        {

            animator.Play("Armature|OpenFreezer"); // start at start
            isOpen = true;
        }
    }
}
