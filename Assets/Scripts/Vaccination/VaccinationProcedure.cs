using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VaccinationProcedure : MonoBehaviour
{
    [Header("Toggles for Vaccination Choice")]
    public Toggle distemperToggle;
    public Toggle bordetellaToggle;
    public Toggle leptoToggle;
    public Toggle rabiesToggle;

    [Header("Scene Names")]
    public string distemperScene = "StoryMode_DAP";
    public string bordetellaScene = "StoryMode_Bordetella";
    public string leptoScene = "StoryMode_Lepto";
    public string rabiesScene = "StoryMode_Rabies";

    void Start()
    {
        // Initially disable toggles until DAP is done
        bool dapDone = ChecklistManager.Instance?.IsTaskCompleted("Vaccinate the dog") ?? false;

        distemperToggle.interactable = false; // Already done
        bordetellaToggle.interactable = dapDone;
        leptoToggle.interactable = dapDone;
        rabiesToggle.interactable = dapDone;

        // Add listeners
        bordetellaToggle.onValueChanged.AddListener(delegate { OnToggleSelected(bordetellaToggle, bordetellaScene); });
        leptoToggle.onValueChanged.AddListener(delegate { OnToggleSelected(leptoToggle, leptoScene); });
        rabiesToggle.onValueChanged.AddListener(delegate { OnToggleSelected(rabiesToggle, rabiesScene); });
    }

    private void OnToggleSelected(Toggle toggle, string sceneName)
    {
        if (toggle.isOn)
        {
            // Prevent multiple selections
            bordetellaToggle.interactable = false;
            leptoToggle.interactable = false;
            rabiesToggle.interactable = false;

            Debug.Log($"Loading scene for {sceneName}...");
            SceneManager.LoadScene(sceneName);
        }
    }

    // Optional: Call this to refresh interactable states if DAP is completed dynamically
    public void RefreshToggles()
    {
        bool dapDone = ChecklistManager.Instance?.IsTaskCompleted("Vaccinate the dog") ?? false;

        bordetellaToggle.interactable = dapDone;
        leptoToggle.interactable = dapDone;
        rabiesToggle.interactable = dapDone;
    }
}
