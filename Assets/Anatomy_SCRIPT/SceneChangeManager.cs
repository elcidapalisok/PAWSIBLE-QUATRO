using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SceneChangeManager : MonoBehaviour
{
    [Header("Next Scene")]
    public string nextSceneName; // Assign in Inspector

    // This method can be called by a Timeline Signal
    public void TriggerNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next Scene Name is not assigned!");
        }
    }
}
