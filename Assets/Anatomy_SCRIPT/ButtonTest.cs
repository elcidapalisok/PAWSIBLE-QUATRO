using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRPanelButton : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject mainPanel;
    public GameObject detailedPanel;
    
    private XRSimpleInteractable interactable;
    private Button button;

    void Start()
    {
        // Get components
        interactable = GetComponent<XRSimpleInteractable>();
        button = GetComponent<Button>();
        
        // Setup VR interaction
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnVRButtonPressed);
        }
        
        // Also keep regular button functionality as backup
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    void OnVRButtonPressed(SelectEnterEventArgs args)
    {
        Debug.Log("VR Controller pressed the button!");
        SwitchPanels();
    }

    void OnButtonClicked()
    {
        Debug.Log("Traditional button click");
        SwitchPanels();
    }

    void SwitchPanels()
    {
        if (mainPanel != null && detailedPanel != null)
        {
            bool isDetailedActive = detailedPanel.activeInHierarchy;
            
            mainPanel.SetActive(!isDetailedActive);
            detailedPanel.SetActive(isDetailedActive);
            
            Debug.Log($"Switched panels - Main: {mainPanel.activeSelf}, Detailed: {detailedPanel.activeSelf}");
        }
    }
}