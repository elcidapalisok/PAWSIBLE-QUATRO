// AnatomyIconToggle.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class AnatomyIconToggle : MonoBehaviour
{
    [Header("Anatomy Object Info")]
    public string targetAnatomyName; // Name that matches with AnatomyManager
    
    private Toggle toggle;
    private bool isInitialized = false;
    
    void Awake()
    {
        toggle = GetComponent<Toggle>();
        
        // Set up the toggle callback
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        
        // Add a Toggle Group if not present
        if (transform.parent != null && transform.parent.GetComponent<ToggleGroup>() == null)
        {
            ToggleGroup group = transform.parent.gameObject.AddComponent<ToggleGroup>();
            group.allowSwitchOff = true;
        }
        
        // Assign this toggle to the group
        if (transform.parent != null)
        {
            ToggleGroup group = transform.parent.GetComponent<ToggleGroup>();
            if (group != null)
            {
                toggle.group = group;
            }
        }
        
        isInitialized = true;
    }
    
    void Start()
    {
        // Initialize toggle state
        if (toggle.isOn)
        {
            OnToggleValueChanged(true);
        }
    }
    
    private void OnToggleValueChanged(bool isOn)
    {
        if (!isInitialized) return;
        
        if (isOn && !string.IsNullOrEmpty(targetAnatomyName))
        {
            if (AnatomyManager.Instance != null)
            {
                AnatomyManager.Instance.HighlightObject(targetAnatomyName);
            }
            else
            {
                Debug.LogError("AnatomyManager instance not found!");
            }
        }
        else if (!isOn)
        {
            // When toggle is turned off, remove highlight from this object
            if (AnatomyManager.Instance != null && !string.IsNullOrEmpty(targetAnatomyName))
            {
                // Get the specific object and remove its highlight
                var anatomyObj = AnatomyManager.Instance.GetAnatomyObject(targetAnatomyName);
                if (anatomyObj != null && anatomyObj.highlighter != null)
                {
                    anatomyObj.highlighter.RemoveHighlight();
                }
            }
        }
    }
    
    // Public method to programmatically select this icon
    public void SelectIcon()
    {
        if (toggle != null)
        {
            toggle.isOn = true;
        }
    }
    
    // Public method to deselect this icon
    public void DeselectIcon()
    {
        if (toggle != null)
        {
            toggle.isOn = false;
        }
    }
    
    void OnDestroy()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}