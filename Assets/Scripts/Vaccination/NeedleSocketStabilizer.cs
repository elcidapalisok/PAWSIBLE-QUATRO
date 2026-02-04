using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class NeedleSocketStabilizer : MonoBehaviour
{
    [Header("Assign the syringe body colliders to ignore while needle is socketed")]
    public Collider[] syringeBodyColliders;

    private XRSocketInteractor socket;

    // Store original collision states so we can restore them
    private readonly List<(Collider a, Collider b)> ignoredPairs = new();

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnSocketed);
        socket.selectExited.AddListener(OnUnsocketed);
    }

    void OnDestroy()
    {
        socket.selectEntered.RemoveListener(OnSocketed);
        socket.selectExited.RemoveListener(OnUnsocketed);
    }

    private void OnSocketed(SelectEnterEventArgs args)
    {
        var needleTransform = args.interactableObject.transform;
        var needleRB = needleTransform.GetComponent<Rigidbody>();
        if (needleRB != null)
        {
            // Make needle "part of the syringe" while attached
            needleRB.linearVelocity = Vector3.zero;
            needleRB.angularVelocity = Vector3.zero;
            needleRB.isKinematic = true;
            needleRB.useGravity = false;
        }

        // Ignore collisions between needle colliders and syringe body colliders while attached
        var needleColliders = needleTransform.GetComponentsInChildren<Collider>();
        foreach (var nCol in needleColliders)
        {
            if (nCol == null || nCol.isTrigger) continue;

            foreach (var bCol in syringeBodyColliders)
            {
                if (bCol == null) continue;

                Physics.IgnoreCollision(nCol, bCol, true);
                ignoredPairs.Add((nCol, bCol));
            }
        }
    }

    private void OnUnsocketed(SelectExitEventArgs args)
    {
        var needleTransform = args.interactableObject.transform;
        var needleRB = needleTransform.GetComponent<Rigidbody>();
        if (needleRB != null)
        {
            // Restore physics so the user can carry it away
            needleRB.isKinematic = false;
            // needleRB.useGravity = true; // enable if you want it to fall when removed
        }

        // Restore collisions
        foreach (var (a, b) in ignoredPairs)
        {
            if (a != null && b != null)
                Physics.IgnoreCollision(a, b, false);
        }
        ignoredPairs.Clear();
    }
}
