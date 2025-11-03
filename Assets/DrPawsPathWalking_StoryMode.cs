using UnityEngine;
using System.Collections;

public class DrPawsPathWalker_Storymode : MonoBehaviour
{
    [Header("XR Socket Settings")]
    public GameObject xrSocket;      // Assign your XR socket here
    public GameObject ChecklistObject;  

    [Header("Path Settings")]
    public Transform[] pathPoints;
    [Range(0.1f, 5f)] public float speed = 1.2f;
    public float reachDistance = 0.2f;
    public float rotationSpeed = 2f;

    [Header("Animation Settings")]
    public Animator animator;

    private int currentPoint = 1;
    private bool isMoving = false;
    private bool canMove = false;
    private bool isPaused = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (pathPoints.Length > 0)
        {
            transform.position = pathPoints[0].position;
            Debug.Log($"✅ Starting at {pathPoints[0].name}");
        }
        else
        {
            Debug.LogWarning("⚠️ No path points assigned!");
        }

        animator?.SetBool("isWalking", false);
    }

    void Update()
    {
        if (!canMove || isPaused) return;

        if (pathPoints.Length == 0 || currentPoint >= pathPoints.Length)
        {
            if (isMoving)
            {
                isMoving = false;
                animator?.SetBool("isWalking", false);
                Debug.Log("🏁 Finished walking path!");
            }
            return;
        }

        Transform targetPoint = pathPoints[currentPoint];
        Vector3 direction = targetPoint.position - transform.position;
        float distance = direction.magnitude;

        if (distance > reachDistance)
        {
            // Move and rotate
            Vector3 moveDirection = direction.normalized;
            transform.position += moveDirection * speed * Time.deltaTime;

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            if (!isMoving)
            {
                isMoving = true;
                animator?.SetBool("isWalking", true);
                Debug.Log($"🚶 Walking toward {targetPoint.name}");
            }
        }
        else
        {
            Debug.Log($"✅ Reached {targetPoint.name}");
            StartCoroutine(HandlePointReachedWithDelay(currentPoint));
            currentPoint++;
        }
    }

    private IEnumerator HandlePointReachedWithDelay(int index)
    {
        isPaused = true;
        isMoving = false;
        animator?.SetBool("isWalking", false);

        // Make Dr. Paws face the current object
        Transform lookTarget = pathPoints[index];
        if (lookTarget != null)
        {
            Vector3 lookDirection = (lookTarget.position - transform.position).normalized;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = lookRotation;
            }
        }

        switch (index)
        {

            case 1:
                 Debug.Log("🦴 CHECKPOINT1");
                break;

            case 2:
          Debug.Log("🦴 CHECKPOINT2");

                yield return new WaitForSeconds(0);
                break;

            case 3:
                Debug.Log("🦴 CHECKPOINT3");
                break;

     

        
        }

        // Resume walking if not finished
        if (index < pathPoints.Length - 1)
        {
            isPaused = false;
            animator?.SetBool("isWalking", true);
        }
        else
        {
            animator?.SetBool("isWalking", false);
            canMove = false; // Stop permanently
        }
    }

    // ✅ Timeline triggers
    public void StartWalkingFromTimeline()
    {
        canMove = true;
        animator?.SetBool("isWalking", true);
        animator?.SetBool("isGrabBone", false);
        animator?.SetBool("isGreetings", false);
        animator?.SetBool("isGiving", false);
        animator?.SetBool("isTalking", false);
      
        Debug.Log("🎬 Timeline Trigger: Dr. Paws starts moving!");
    }

    public void Greetings()
    {

        animator?.SetBool("isGreetings", true);


    }
    public void OpeningDoor()
    {

        animator?.SetBool("isOpeningDoor", true);

    }
    public void GiveChecklist()
    {

        animator?.SetBool("isGiving", true);

    }
            public void Idle()
    {
        animator?.SetBool("isIdle", true);
        animator?.SetBool("isGiving", false);

    }
    public void StopWalkingFromTimeline()
    {
        canMove = false;
        isMoving = false;
        animator?.SetBool("isWalking", false);
        Debug.Log("⏸️ Timeline Trigger: Dr. Paws stops moving!");
    }
    public void ResumeWalking()
    {
        Debug.Log("▶ Resuming walking!");
        canMove = true;
        isPaused = false;
        animator?.SetBool("isWalking", true);

    }
    public void Clapping()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isGrabbing", false);
        animator.SetBool("isGiving", false);
        animator.SetBool("isOpeningDoor", false);

        animator.SetBool("isClapping", true);
    }

    public void Talking()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isGrabbing", false);
        animator.SetBool("isGiving", false);
        animator.SetBool("isOpeningDoor", false);
        animator.SetBool("isClapping", false);

        animator.SetBool("isTalking", true);

    }
  public void Angry()
{
    animator.SetBool("isAngry", true);

}




    void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Length < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            if (pathPoints[i] != null && pathPoints[i + 1] != null)
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
        }
    }
}
