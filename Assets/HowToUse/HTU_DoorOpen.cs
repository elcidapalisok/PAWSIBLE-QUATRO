using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class HTU_DoorOpen : MonoBehaviour
{
    [Header("Next Scene")]
    public string nextSceneName; // Assign in Inspector

    private XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnDoorClicked);
    }

    private void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnDoorClicked);
    }

    private void OnDoorClicked(SelectEnterEventArgs args)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
