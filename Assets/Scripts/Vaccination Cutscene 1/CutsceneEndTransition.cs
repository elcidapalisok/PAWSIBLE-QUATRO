using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneEndTransition : MonoBehaviour
{
    [Header("Scene Transition")]
    [Tooltip("Exact name of the scene to load after the cutscene ends")]
    public string nextSceneName = "StoryMode Vaccine";

    // This method will be called by the Signal Receiver
    public void OnCutsceneEndSignal()
    {
        Debug.Log($"Cutscene finished — loading scene: {nextSceneName}");

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("[CutsceneEndTransition] No scene specified to load.");
        }
    }
}
