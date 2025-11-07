using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    public PlayableDirector director;
    public DialogueManager dialogueManager;

    void OnEnable()
    {
        director.stopped += OnCutsceneEnd;
        dialogueManager.cutsceneMode = true;
    }

    void OnCutsceneEnd(PlayableDirector pd)
    {
        if (pd == director)
            dialogueManager.cutsceneMode = false;
    }
}
