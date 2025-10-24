using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Linq;

public class DrPawsMultipleXrSocketTagInteractor : XRSocketInteractor
{
    [Header("Accepted Tags")]
    [SerializeField] private string[] targetTags = { "Untagged" }; // You can add multiple tags in the Inspector

    private bool HasValidTag(Transform obj)
    {
        // Checks if object's tag matches any in the array
        return targetTags.Contains(obj.tag);
    }

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && HasValidTag(interactable.transform);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && HasValidTag(interactable.transform);
    }
}
