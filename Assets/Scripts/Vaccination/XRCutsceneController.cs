using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRCutsceneController : MonoBehaviour
{
    public PlayableDirector director;
    public GameObject xrOrigin; // XR Origin or Player Rig
    public DialogueManager dialogueManager;

    private XROrigin origin;
    private ActionBasedContinuousMoveProvider moveProvider;
    private ActionBasedContinuousTurnProvider turnProvider;
    private XRRayInteractor[] rayInteractors;

    void Awake()
    {
        if (xrOrigin != null)
        {
            origin = xrOrigin.GetComponent<XROrigin>();
            moveProvider = xrOrigin.GetComponentInChildren<ActionBasedContinuousMoveProvider>();
            turnProvider = xrOrigin.GetComponentInChildren<ActionBasedContinuousTurnProvider>();
            rayInteractors = xrOrigin.GetComponentsInChildren<XRRayInteractor>(true);
        }
    }

    void OnEnable()
    {
        director.played += OnCutsceneStart;
        director.stopped += OnCutsceneEnd;
    }

    void OnDisable()
    {
        director.played -= OnCutsceneStart;
        director.stopped -= OnCutsceneEnd;
    }

    void OnCutsceneStart(PlayableDirector pd)
    {
        if (xrOrigin != null)
        {
            // Disable controllers only, not the camera
            var left = xrOrigin.transform.Find("LeftHand Controller");
            var right = xrOrigin.transform.Find("RightHand Controller");
            if (left) left.gameObject.SetActive(false);
            if (right) right.gameObject.SetActive(false);
        }

        if (dialogueManager != null)
            dialogueManager.cutsceneMode = true;
    }


    void OnCutsceneEnd(PlayableDirector pd)
    {
        // Re-enable controls after cutscene
        if (moveProvider) moveProvider.enabled = true;
        if (turnProvider) turnProvider.enabled = true;
        if (rayInteractors != null)
        {
            foreach (var ray in rayInteractors)
                ray.enabled = true;
        }

        if (dialogueManager != null)
            dialogueManager.cutsceneMode = false;

        Debug.Log("[XRCutsceneController] Cutscene ended — controls restored.");
    }
}
