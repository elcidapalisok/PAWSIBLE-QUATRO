using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class RadialPartXR : MonoBehaviour
{
    [SerializeField] private Image sliceImage;
    [SerializeField] private TextMeshProUGUI label;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.cyan;

    private XRBaseInteractable interactable;
    private RadialSelection menu;
    private int index;

    void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        menu = GetComponentInParent<RadialSelection>();
        sliceImage.color = normalColor;
    }

    void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        interactable.selectEntered.AddListener(OnSelect);
    }

    void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
        interactable.selectEntered.RemoveListener(OnSelect);
    }

    public void Setup(int sliceIndex, string text)
    {
        index = sliceIndex;
        label.text = text;
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        sliceImage.color = hoverColor;
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        sliceImage.color = normalColor;
    }

    void OnSelect(SelectEnterEventArgs args)
    {
        menu.OnSliceSelected(index);
    }
}
