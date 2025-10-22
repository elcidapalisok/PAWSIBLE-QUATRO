using UnityEngine;
using System.Collections;

public class MoveUpAndRotateTrigger : MonoBehaviour
{
    [Header("References")]
    public string playerTag = "Player"; // your DrPaws tag
    public Transform targetObject;      // object to move

    [Header("Movement Settings")]
    public float moveHeight = 2f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 180f;

    [Header("Testing")]
    public bool onStartTest = false;    // for quick testing in Play Mode

    private bool isAnimating = false;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Quaternion originalRotation;

    private void Start()
    {
        if (targetObject == null) targetObject = transform;

        startPos = targetObject.position;
        targetPos = startPos + Vector3.up * moveHeight;
        originalRotation = targetObject.rotation;

        // 🔹 Automatically test the motion when play starts
        if (onStartTest)
        {
            StopAllCoroutines();
            StartCoroutine(MoveUpAndRotate());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            StopAllCoroutines();
            StartCoroutine(MoveUpAndRotate());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            StopAllCoroutines();
            StartCoroutine(MoveDown());
        }
    }

    private IEnumerator MoveUpAndRotate()
    {
        isAnimating = true;
        float elapsed = 0f;
        float duration = moveHeight / moveSpeed;

        while (elapsed < duration)
        {
            // Move upward
            targetObject.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);

            // Spin while moving
            targetObject.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final position and restore rotation
        targetObject.position = targetPos;
        targetObject.rotation = originalRotation;
        isAnimating = false;
    }

    private IEnumerator MoveDown()
    {
        isAnimating = true;
        float elapsed = 0f;
        float duration = moveHeight / moveSpeed;

        while (elapsed < duration)
        {
            // Move downward
            targetObject.position = Vector3.Lerp(targetPos, startPos, elapsed / duration);

            // Spin back down
            targetObject.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime, Space.World);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final position and rotation
        targetObject.position = startPos;
        targetObject.rotation = originalRotation;
        isAnimating = false;
    }
}
