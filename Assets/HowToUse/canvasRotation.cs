using UnityEngine;
using System.Collections;

public class MoveUpAndRotateTrigger : MonoBehaviour
{
    [Header("References")]
    public string playerTag = "Player"; // XR Origin or player collider tag
    public Transform targetObject;     

    [Header("Movement Settings")]
    public float moveHeight = 2f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 180f;

    private bool isAnimating = false;
    private bool playerInside = false;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Quaternion startRot;

    private void Start()
    {
        if (targetObject == null) targetObject = transform;

        startPos = targetObject.position;
        targetPos = startPos + Vector3.up * moveHeight;
        startRot = targetObject.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ✅ Detect XR child collider
        if (!other.CompareTag(playerTag)) return;
        if (playerInside) return;

        Debug.Log("✅ Player ENTER detected!");
        playerInside = true;

        StopAllCoroutines();
        StartCoroutine(MoveUpAndRotate());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (!playerInside) return;

        Debug.Log("⬇ Player EXIT detected!");
        playerInside = false;

        StopAllCoroutines();
        StartCoroutine(MoveDown());
    }

    private IEnumerator MoveUpAndRotate()
    {
        isAnimating = true;
        float elapsed = 0f;
        float duration = moveHeight / moveSpeed;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            targetObject.position = Vector3.Lerp(startPos, targetPos, t);
            targetObject.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

            elapsed += Time.deltaTime;
            yield return null;
        }

        targetObject.position = targetPos;
        isAnimating = false;
    }

    private IEnumerator MoveDown()
    {
        isAnimating = true;
        float elapsed = 0f;
        float duration = moveHeight / moveSpeed;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            targetObject.position = Vector3.Lerp(targetPos, startPos, t);
            targetObject.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime, Space.World);

            elapsed += Time.deltaTime;
            yield return null;
        }

        targetObject.position = startPos;
        targetObject.rotation = startRot;
        isAnimating = false;
    }
}
