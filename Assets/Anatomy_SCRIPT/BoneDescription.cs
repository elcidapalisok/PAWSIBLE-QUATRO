using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class BoneDescription : MonoBehaviour
{
    [Header("Bone Info - English")]
    public string boneTitle;           // e.g. "Femur"
    [TextArea] public string boneInfo; // detailed description

    [Header("Bone Info - Tagalog")]
    public string boneTitleTagalog;    // e.g. "Hita"
    [TextArea] public string boneInfoTagalog; // Tagalog description

    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    [Header("Default Messages")]
    [TextArea] public string defaultTitleEnglish = "Skeletal structure";
    [TextArea] public string defaultMessageEnglish = "Welcome to PAWSIBLE! This is the skeletal system of the dog. The skeleton provides structure, protects vital organs, and supports movement. Explore each bone to learn its name, location, and function";

    [TextArea] public string defaultTitleTagalog = "Estruktura ng Kalansay";
    [TextArea] public string defaultMessageTagalog = "Maligayang pagdating sa PAWSIBLE! Ito ang skeletal system ng aso. Ang kalansay ay nagbibigay ng istruktura, nangangalaga ng mga mahalagang organo, at sumusuporta sa pagkilos. Tuklasin ang bawat buto upang malaman ang pangalan, lokasyon, at tungkulin nito";

    private XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;
    private bool ready = false; // prevents early triggering
    
    // Language state - CHANGED: Make setter public
    private bool isTagalog = false;
    public static bool IsTagalogMode { get; set; } = false; // Removed "private set;"

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.hoverEntered.AddListener(OnHoverEnter);
        grabInteractable.hoverExited.AddListener(OnHoverExit);
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void Start()
    {
        // Initialize with current language mode
        isTagalog = IsTagalogMode;
        
        // Delay one frame to prevent auto-trigger on scene start
        StartCoroutine(EnableAfterDelay());
    }

    IEnumerator EnableAfterDelay()
    {
        yield return null; // wait 1 frame
        ready = true;
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (!ready || isGrabbed) return;
        ShowText();
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (!ready || isGrabbed) return;
        ClearText();
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!ready) return;
        isGrabbed = true;
        ShowText();
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (!ready) return;
        isGrabbed = false;
        ClearText();
    }

    private void ShowText()
    {
        if (titleText != null)
        {
            titleText.text = isTagalog ? boneTitleTagalog : boneTitle;
        }
        
        if (descriptionText != null)
        {
            descriptionText.text = isTagalog ? boneInfoTagalog : boneInfo;
        }
    }

    private void ClearText()
    {
        if (titleText != null)
        {
            titleText.text = isTagalog ? defaultTitleTagalog : defaultTitleEnglish;
        }
        
        if (descriptionText != null)
        {
            descriptionText.text = isTagalog ? defaultMessageTagalog : defaultMessageEnglish;
        }
    }

    // Public method to toggle language - ONLY updates this bone
    public void ToggleLanguage()
    {
        // Sync with global language mode
        isTagalog = IsTagalogMode;
        
        // Only update display if this bone is currently grabbed
        if (isGrabbed)
        {
            ShowText();
        }
    }

    // Public method to set language explicitly
    public void SetLanguage(bool useTagalog)
    {
        // This method should only update this bone's display
        // The global mode is set by the LanguageToggleButton
        isTagalog = useTagalog;
        
        // Only update display if this bone is currently grabbed
        if (isGrabbed)
        {
            ShowText();
        }
    }
}