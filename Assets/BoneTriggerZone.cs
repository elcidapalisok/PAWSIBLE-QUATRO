using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class BoneTriggerZone : MonoBehaviour
{
    [Header("Primary Correct Bone Tag")]
    public string PrimaryTargetTag = "PrimaryBone";

    [Header("Other Allowed Bone Tags")]
    public List<string> OtherTargetTags = new List<string>();

    [Header("Events")]
    public UnityEvent<GameObject> OnPrimaryTargetEnterEvent;
    public UnityEvent<GameObject> OnOtherTargetEnterEvent;


private void OnTriggerEnter(Collider other)
{
    string objTag = other.transform.root.tag;

    if (objTag == PrimaryTargetTag)
    {
        OnPrimaryTargetEnterEvent?.Invoke(other.transform.root.gameObject);
        return;
    }

    if (OtherTargetTags.Contains(objTag))
    {
        OnOtherTargetEnterEvent?.Invoke(other.transform.root.gameObject);
    }
}

}
