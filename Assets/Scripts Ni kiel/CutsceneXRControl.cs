using UnityEngine;
using UnityEngine.Playables;

public class CutsceneXRControl : MonoBehaviour
{
    public PlayableDirector director;
    public Behaviour trackedPoseDriver; // Drag component here

    void Start()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();

        // Disable head tracking so Cinemachine controls camera
        if (trackedPoseDriver != null)
            trackedPoseDriver.enabled = false;

        // When timeline finishes, re-enable tracking
        if (director != null)
            director.stopped += OnCutsceneFinished;
    }

    void OnCutsceneFinished(PlayableDirector d)
    {
        if (trackedPoseDriver != null)
            trackedPoseDriver.enabled = true;
    }
}
