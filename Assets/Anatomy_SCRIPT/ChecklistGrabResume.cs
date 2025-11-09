using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ChecklistGrabResume : MonoBehaviour
{
    [Header("Dr. Paws Walker Script")]
    public DrPawsPathWalker_Storymode drPawsWalker;

    private int grabCount = 0;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogError("❌ XRGrabInteractable missing on Checklist object!");
        }
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        grabCount++;
        Debug.Log($"📌 Checklist Grabbed Count: {grabCount}");

        if (grabCount == 2) // ✅ Only on second grab
        {
            Debug.Log("✅ Second grab detected → Dr. Paws resumes walking!");
            drPawsWalker.ResumeWalking();
            
        }
    }
}
