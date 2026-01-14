using UnityEngine;
using UnityEngine.UI;

public class DogRadialMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button btnSit;
    [SerializeField] private Button btnLay;
    [SerializeField] private Button btnPaw;
    [SerializeField] private Button btnCancel;
    [SerializeField] private Button btnCarry;

    [Header("Targets")]
    [SerializeField] private DogAnimator dogAnimator;
    [SerializeField] private DogInteractionMenu dogMenu;

    private void Awake()
    {
        // Auto-find if not assigned
        if (dogAnimator == null) dogAnimator = FindObjectOfType<DogAnimator>(true);
        if (dogMenu == null) dogMenu = FindObjectOfType<DogInteractionMenu>(true);
    }

    private void Start()
    {
        if (btnSit != null) btnSit.onClick.AddListener(OnSit);
        if (btnLay != null) btnLay.onClick.AddListener(OnLay);
        if (btnPaw != null) btnPaw.onClick.AddListener(OnPaw);
        if (btnCancel != null) btnCancel.onClick.AddListener(OnCancel);
        if (btnCarry != null) btnCarry.onClick.AddListener(OnCarry);
    }

    private void OnSit()
    {
        if (dogAnimator == null) return;
        dogAnimator.Sit();
        Debug.Log("Sit clicked");

    }

    private void OnLay()
    {
        if (dogAnimator == null) return;
        dogAnimator.Lay();
        Debug.Log("Lay clicked");

    }

    private void OnPaw()
    {
        if (dogAnimator == null) return;
        dogAnimator.Paw();
        Debug.Log("Paw clicked");

    }

    private void OnCancel()
    {
        if (dogMenu == null) return;
        dogMenu.CloseMenu();
        Debug.Log("Cxl clicked");

    }

    private void OnCarry()
    {
        // Not implemented yet
    }
}
