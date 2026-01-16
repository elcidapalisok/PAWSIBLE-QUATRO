using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class TwoHandScaleStable : MonoBehaviour
{
    public Transform secondHandGrab;

    private XRGrabInteractable grab;
    private Transform firstHand;
    private Transform secondHand;

    private float startDistance;
    private Vector3 startScale;

    public float minScale = 0.2f;
    public float maxScale = 3f;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (firstHand == null)
        {
            firstHand = args.interactorObject.transform;
        }
        else if (secondHand == null)
        {
            secondHand = args.interactorObject.transform;

            startDistance = Vector3.Distance(firstHand.position, secondHand.position);
            startScale = transform.localScale;
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (args.interactorObject.transform == firstHand)
        {
            firstHand = secondHand;
            secondHand = null;
        }
        else if (args.interactorObject.transform == secondHand)
        {
            secondHand = null;
        }
    }

    void Update()
    {
        if (firstHand == null || secondHand == null)
            return;

        float currentDistance = Vector3.Distance(firstHand.position, secondHand.position);
        if (currentDistance <= 0.0001f) return;

        float factor = currentDistance / startDistance;
        Vector3 target = startScale * factor;

        float clamped = Mathf.Clamp(target.x, minScale, maxScale);
        transform.localScale = Vector3.one * clamped;
    }
}
