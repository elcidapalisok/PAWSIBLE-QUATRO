using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DogAnimatorController : MonoBehaviour
{
    private Animator animator;
    private Transform xrHead;

    [Header("Distance Control")]
    [SerializeField] private float farDistance = 3f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        xrHead = Camera.main.transform;

        animator.SetInteger("Action", -1);
        animator.SetBool("IsFar", true);
    }

    void Update()
    {
        float distance = Vector3.Distance(xrHead.position, transform.position);
        bool isFar = distance > farDistance;

        animator.SetBool("IsFar", isFar);

        // If far, always revert to idle cycle
        if (isFar)
        {
            animator.SetInteger("Action", -1);
        }
    }

    public void PlayAction(int actionIndex)
    {
        animator.SetInteger("Action", actionIndex);
        animator.SetTrigger("Interact");
        animator.SetBool("IsFar", false);
    }
}
