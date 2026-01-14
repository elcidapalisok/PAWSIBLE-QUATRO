using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class WaterTapXR : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator tapAnimator;
    [SerializeField] private string openParam = "Open";
    [SerializeField] private string closeParam = "Close";

    [Header("Effects")]
    [SerializeField] private ParticleSystem runningWater;
    [SerializeField] private AudioSource waterSound;

    [Header("State (Read Only)")]
    [SerializeField] private bool isOpen = false;

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnSelect);

        // Force CLOSED state on startup
        ForceClosedState();
    }

    private void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelect);
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        ToggleTap();
    }

    private void ToggleTap()
    {
        SetTapState(!isOpen);
    }

    private void SetTapState(bool open)
    {
        isOpen = open;

        // ---- ANIMATOR ----
        if (tapAnimator != null)
        {
            tapAnimator.SetBool(openParam, isOpen);
            tapAnimator.SetBool(closeParam, !isOpen);
        }

        // ---- WATER PARTICLES ----
        if (runningWater != null)
        {
            if (isOpen)
            {
                if (!runningWater.isPlaying)
                    runningWater.Play(true);
            }
            else
            {
                runningWater.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear
                );
            }
        }

        // ---- SOUND ----
        if (waterSound != null)
        {
            if (isOpen)
            {
                if (!waterSound.isPlaying)
                    waterSound.Play();
            }
            else
            {
                waterSound.Stop();
            }
        }
    }

    private void ForceClosedState()
    {
        isOpen = false;

        if (tapAnimator != null)
        {
            tapAnimator.SetBool(openParam, false);
            tapAnimator.SetBool(closeParam, true);
            tapAnimator.Update(0f); // forces immediate animator sync
        }

        if (runningWater != null)
        {
            runningWater.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }

        if (waterSound != null)
        {
            waterSound.Stop();
        }
    }
}
//