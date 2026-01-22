using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System;

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
    public bool IsOpen => isOpen;

    public event Action<bool> OnTapStateChanged;

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnSelect);
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

        if (tapAnimator != null)
        {
            tapAnimator.SetBool(openParam, isOpen);
            tapAnimator.SetBool(closeParam, !isOpen);
        }

        if (runningWater != null)
        {
            if (isOpen) runningWater.Play(true);
            else runningWater.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (waterSound != null)
        {
            if (isOpen)
            {
                if (!waterSound.isPlaying) waterSound.Play();
            }
            else
            {
                waterSound.Stop();
            }
        }

        OnTapStateChanged?.Invoke(isOpen);
    }

    private void ForceClosedState()
    {
        SetTapState(false);

        if (tapAnimator != null)
            tapAnimator.Update(0f);
    }
}
