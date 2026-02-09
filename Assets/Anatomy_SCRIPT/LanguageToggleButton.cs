using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LanguageToggleButton : MonoBehaviour
{
    [Header("UI References")]
    public Button languageToggleButton;
    public TextMeshProUGUI buttonText;
    
    [Header("Button Text")]
    public string englishText = "Switch to Tagalog";
    public string tagalogText = "Lumipat sa English";

    void Start()
    {
        // Set up button click listener
        if (languageToggleButton != null)
        {
            languageToggleButton.onClick.AddListener(ToggleLanguage);
        }
        
        // Initialize button text
        UpdateButtonText();
    }

    public void ToggleLanguage()
    {
        // Toggle the global language mode
        BoneDescription.IsTagalogMode = !BoneDescription.IsTagalogMode;
        
        // Update ALL bones with the new language setting
        BoneDescription[] allBones = FindObjectsOfType<BoneDescription>();
        foreach (var bone in allBones)
        {
            // This will only update the display if the bone is currently grabbed
            bone.SetLanguage(BoneDescription.IsTagalogMode);
        }
        
        // Update button text
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = BoneDescription.IsTagalogMode ? tagalogText : englishText;
        }
    }
    
    void OnDestroy()
    {
        // Clean up listener
        if (languageToggleButton != null)
        {
            languageToggleButton.onClick.RemoveListener(ToggleLanguage);
        }
    }
}