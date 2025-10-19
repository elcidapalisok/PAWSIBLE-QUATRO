using UnityEngine;

public class DrPawsPathWalker : MonoBehaviour
{
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
    private bool hasGrabbedStick = false; 
    private bool hasPutBone = false;

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
        if (!canMove) return;

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
            currentPoint++;
            isMoving = false;
            animator?.SetBool("isWalking", false);
        }
    }

    // ✅ Triggered by Timeline or other event
   

    public void StartWalkingFromTimeline()
    {
        canMove = true;
        Debug.Log("🎬 Timeline Trigger: Dr. Paws starts moving!");
    }

    public void StopWalkingFromTimeline()
    {
        canMove = false;
        isMoving = false;
        animator?.SetBool("isWalking", false);
        Debug.Log("⏸️ Timeline Trigger: Dr. Paws stops moving!");
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
