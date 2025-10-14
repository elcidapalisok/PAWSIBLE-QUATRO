using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector timeline1;
    public PlayableDirector timeline2;

         public GameObject xrOrigin;

    void Start()
    {
        timeline1.stopped += OnFirstTimelineStopped;
        timeline1.Play();
    }

    void OnFirstTimelineStopped(PlayableDirector director)
    {
        timeline2.Play();
    }
  

    public void DisableXR()
    {
        xrOrigin.SetActive(false);
    }

    public void EnableXR()
    {
        xrOrigin.SetActive(true);
    }
}
