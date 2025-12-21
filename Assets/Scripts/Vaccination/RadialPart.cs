using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class RadialPart : MonoBehaviour
{
    [SerializeField] private Image image;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.blue;

    private XRBaseInteractable interactable;
    private int index;

    void Awake()
    {
        if (!image)
            image = GetComponent<Image>();

        // Store prefab color as default if not overridden
        if (normalColor == Color.white)
            normalColor = image.color;

        image.color = normalColor;

        interactable = GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        interactable.selectEntered.AddListener(OnSelect);
    }

    public void Initialize(int i)
    {
        index = i;
    }

    public void SetFill(float fillAmount)
    {
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Radial360;
        image.fillOrigin = (int)Image.Origin360.Top;
        image.fillClockwise = true;
        image.fillAmount = fillAmount;
    }

    public void SetRotation(float angle)
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        image.color = hoverColor;
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        image.color = normalColor;
    }

    void OnSelect(SelectEnterEventArgs args)
    {
        Debug.Log($"Radial slice selected: {index}");
        // NEXT: trigger dog animation here
    }
}
