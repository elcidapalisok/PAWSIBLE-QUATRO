using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DualHandleGrab : XRGrabInteractable
{
    public Transform leftHandle;
    public Transform rightHandle;

    private IXRSelectInteractor firstInteractor;
    private IXRSelectInteractor secondInteractor;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (firstInteractor == null)
            firstInteractor = args.interactorObject;
        else if (secondInteractor == null)
            secondInteractor = args.interactorObject;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (args.interactorObject == firstInteractor)
            firstInteractor = null;
        else if (args.interactorObject == secondInteractor)
            secondInteractor = null;
    }

    void Update()
    {
        if (firstInteractor != null && secondInteractor != null)
        {
            // Middle point between both controllers
            Vector3 midPos = (firstInteractor.transform.position + secondInteractor.transform.position) / 2f;
            transform.position = midPos;

            // Face forward along direction of hands
            Vector3 direction = secondInteractor.transform.position - firstInteractor.transform.position;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        else if (firstInteractor != null)
        {
            transform.position = firstInteractor.transform.position;
            transform.rotation = firstInteractor.transform.rotation;
        }
    }
}
