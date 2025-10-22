using UnityEngine;
using System.Collections;

public class DrPawsPathWalker_Quiz : MonoBehaviour
{
    [Header("XR Socket Settings")]
public GameObject xrSocket;      // Assign your XR socket here
public GameObject boneObject;    // Assign the bone GameObject here


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

        Transform lookTarget = pathPoints[index]; // so Dr. Paws faces the current object
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
    // 🦴 Play grab animation first
    animator.SetBool("isGrabBone", true);
    Debug.Log("🦴 Dr. Paws starts grabbing bone...");
    yield return new WaitForSeconds(1.2f); // wait before attaching
    
    // Attach bone mid-animation

    Debug.Log("🦴 Bone successfully attached.");

    yield return new WaitForSeconds(1.3f); // small delay to finish grab animation
    animator.SetBool("isGrabBone", false);
    break;


            case 2:


    break;
            case 3:
                    // 🐾 Put the bone
                animator.SetBool("isPuttingBone", true);
                Debug.Log("🐾 Dr. Paws puts the bone.");
                yield return new WaitForSeconds(3f);
                 animator.SetBool("isPuttingBone", false);
                // 🧩 Disable socket and physics control
    if (xrSocket != null)
    {
        xrSocket.SetActive(false); // disable XR socket
        Debug.Log("🧩 XR Socket disabled.");
    }

    // disable isKinematic so bone falls or stays released
    if (boneObject != null)
    {
        Rigidbody rb = boneObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            Debug.Log("💥 Bone physics re-enabled (isKinematic = false).");
        }
    }

    yield return new WaitForSeconds(1.5f);

      break;

          

            case 4:
                // 💤 Idle at final point
                animator.SetBool("isWalking", false);
                Debug.Log("😴 Dr. Paws goes idle at destination.");
                yield return new WaitForSeconds(2f);
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
            canMove = false; // stop permanently
        }
    }

    // ✅ Timeline triggers
    public void StartWalkingFromTimeline()
    {
        canMove = true;
        animator?.SetBool("isWalking", true);
        animator.SetBool("isGrabBone", false);
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
