using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIButtonManager : MonoBehaviour
{
    [System.Serializable]
    public class ButtonStyle
    {
        public Button button;
        public Image background;
        public TextMeshProUGUI label;
    }

    [Header("Buttons")]
    public List<ButtonStyle> buttons;

    [Header("Normal Style")]
    public Sprite normalSprite;
    public Color normalTextColor = Color.black;

    [Header("Selected Style")]
    public Sprite selectedSprite;
    public Color selectedTextColor = Color.white;

    private ButtonStyle currentlySelected;

    private void Start()
    {
        // Assume first button is default selected
        if (buttons.Count > 0)
        {
            currentlySelected = buttons[0];
            SetButtonStyle(currentlySelected, true);
        }

        // Add listeners to all buttons
        foreach (var btnStyle in buttons)
        {
            btnStyle.button.onClick.AddListener(() => OnButtonClicked(btnStyle));
        }
    }

    private void OnButtonClicked(ButtonStyle clicked)
    {
        if (clicked == currentlySelected) return; // already selected

        // Revert previous button
        if (currentlySelected != null)
        {
            SetButtonStyle(currentlySelected, false);
        }

        // Set new selected button
        SetButtonStyle(clicked, true);
        currentlySelected = clicked;
    }

    private void SetButtonStyle(ButtonStyle btnStyle, bool selected)
    {
        if (btnStyle.background != null)
            btnStyle.background.sprite = selected ? selectedSprite : normalSprite;

        if (btnStyle.label != null)
            btnStyle.label.color = selected ? selectedTextColor : normalTextColor;
    }
}
     