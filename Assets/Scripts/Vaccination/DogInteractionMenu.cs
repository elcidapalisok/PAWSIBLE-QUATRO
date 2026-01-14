using UnityEngine;

public class DogInteractionMenu : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject menuRoot;

    [Header("Animator")]
    [SerializeField] private DogAnimator dogAnimator;

    [Header("Follow Settings")]
    [SerializeField] private Transform playerHead;
    [SerializeField] private float followDistance = 0.8f;
    [SerializeField] private float heightOffset = -0.15f;
    [SerializeField] private float maxDistance = 2.5f;
    [SerializeField] private float positionLerp = 15f;
    [SerializeField] private float rotationLerp = 15f;

    private bool isOpen;

    private void Awake()
    {
        if (menuRoot != null)
            menuRoot.SetActive(false);

        if (dogAnimator == null)
            dogAnimator = GetComponent<DogAnimator>();
    }

    private void Update()
    {
        if (!isOpen) return;
        if (menuRoot == null || playerHead == null) return;

        float distToDog = Vector3.Distance(playerHead.position, transform.position);
        if (distToDog > maxDistance)
        {
            CloseMenu();
            return;
        }

        Vector3 forwardFlat = playerHead.forward;
        forwardFlat.y = 0f;
        if (forwardFlat.sqrMagnitude < 0.0001f) forwardFlat = Vector3.forward;
        forwardFlat.Normalize();

        Vector3 targetPos = playerHead.position + forwardFlat * followDistance + Vector3.up * heightOffset;

        menuRoot.transform.position = Vector3.Lerp(menuRoot.transform.position, targetPos, Time.deltaTime * positionLerp);

        Vector3 lookDir = playerHead.position - menuRoot.transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            menuRoot.transform.rotation = Quaternion.Slerp(menuRoot.transform.rotation, targetRot, Time.deltaTime * rotationLerp);
        }
    }

    // Hook this to XR Simple Interactable -> Select Entered
    public void OpenMenuFromDog()
    {
        if (isOpen) return;
        OpenMenu();
    }

    public void OpenMenu()
    {
        isOpen = true;
        if (menuRoot != null) menuRoot.SetActive(true);
        if (dogAnimator != null) dogAnimator.BeginInteract();
    }

    public void CloseMenu()
    {
        isOpen = false;
        if (menuRoot != null) menuRoot.SetActive(false);
        if (dogAnimator != null) dogAnimator.EndInteract();
    }
}
