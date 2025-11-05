using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Sanitizer_Animation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool isOpen = false;

    public void ToggleSanitizer()
    {
        if (isOpen)
        {

            animator.Play("Sanitizer_Press 0"); // start at end
            isOpen = false;
        }
        else
        {

            animator.Play("Sanitizer_Press"); // start at start
            isOpen = true;
        }
    }
}
