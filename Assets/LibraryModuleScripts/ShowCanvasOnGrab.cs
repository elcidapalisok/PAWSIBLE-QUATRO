using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShowCanvasOnGrab : MonoBehaviour
{
    [SerializeField] private GameObject libraryCanvas;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    private void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void Start()
    {
        if (libraryCanvas != null)
            libraryCanvas.SetActive(false); // start hidden
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (libraryCanvas != null)
            libraryCanvas.SetActive(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (libraryCanvas != null)
            libraryCanvas.SetActive(false);
    }
}
