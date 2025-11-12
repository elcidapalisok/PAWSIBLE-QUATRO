using UnityEngine;
using System.Collections;

public class SceneLoadPhysicsDelay : MonoBehaviour
{
    [Header("Delay Settings")]
    public float freezeDuration = 0.5f; // how long to pause physics on scene start

    private IEnumerator Start()
    {
        // ✅ Step 1: Pause all physics for a brief moment
        Physics.autoSimulation = false;

        // ✅ Step 2: Wait for the scene to fully load (colliders, lighting, etc.)
        yield return new WaitForSeconds(freezeDuration);

        // ✅ Step 3: Resume physics simulation normally
        Physics.autoSimulation = true;
    }
}
