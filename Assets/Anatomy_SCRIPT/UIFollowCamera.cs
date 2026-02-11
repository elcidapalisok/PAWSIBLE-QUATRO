using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
    public Transform cameraTransform;
    public float distance = 2f;
    public float smoothSpeed = 5f;

    void Update()
    {
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * distance;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, cameraTransform.rotation, Time.deltaTime * smoothSpeed);
    }
}