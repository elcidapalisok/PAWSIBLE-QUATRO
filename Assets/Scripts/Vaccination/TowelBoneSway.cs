using UnityEngine;

public class TowelBoneSway : MonoBehaviour
{
    [Header("Bones to sway (lower bones only)")]
    public Transform[] bones;

    [Header("Sway Settings")]
    [Range(0f, 10f)] public float swayStrength = 2.0f;
    [Range(0.01f, 30f)] public float damping = 10f;

    [Tooltip("Maximum sway angle in degrees.")]
    [Range(0f, 45f)] public float maxAngle = 12f;

    [Header("Source (Optional)")]
    public Rigidbody sourceRigidbody;

    private Quaternion[] restRotations;

    private void Awake()
    {
        if (bones == null || bones.Length == 0) return;

        restRotations = new Quaternion[bones.Length];
        for (int i = 0; i < bones.Length; i++)
            restRotations[i] = bones[i].localRotation;

        if (sourceRigidbody == null)
            sourceRigidbody = GetComponentInParent<Rigidbody>();
    }

    private void LateUpdate()
    {
        if (bones == null || bones.Length == 0) return;

        Vector3 vel = sourceRigidbody != null ? sourceRigidbody.linearVelocity : Vector3.zero;

        // Convert velocity to local space; sway opposite of motion
        Vector3 localVel = transform.InverseTransformDirection(vel);
        float swayX = Mathf.Clamp(-localVel.z * swayStrength, -maxAngle, maxAngle); // forward/back
        float swayZ = Mathf.Clamp(localVel.x * swayStrength, -maxAngle, maxAngle);  // left/right

        for (int i = 0; i < bones.Length; i++)
        {
            // Lower bones sway more than upper ones
            float t = (bones.Length == 1) ? 1f : (float)i / (bones.Length - 1);
            float boneAngleX = swayX * t;
            float boneAngleZ = swayZ * t;

            Quaternion target = restRotations[i] * Quaternion.Euler(boneAngleX, 0f, boneAngleZ);
            bones[i].localRotation = Quaternion.Slerp(bones[i].localRotation, target, Time.deltaTime * damping);
        }
    }
}
