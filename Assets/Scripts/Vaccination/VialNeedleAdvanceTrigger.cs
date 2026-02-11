using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Collider))]
public class VialNeedleAdvanceTrigger : MonoBehaviour
{
    [Header("Needle Detection")]
    [Tooltip("Tag used on the needle tip collider.")]
    public string needleTipTag = "NeedleTip";

    [Header("Gates (per dialogue line)")]
    public List<Gate> gates = new List<Gate>();

    [Serializable]
    public class Gate
    {
        [Header("Dialogue Gate")]
        public string targetSegmentName = "Vaccine Prep";
        public int targetLineIndex = 0;

        [Header("Optional Syringe Animation")]
        public bool playSyringeAnimation = true;
        public SyringeAnimMode animationMode = SyringeAnimMode.Fill;

        [Header("Behavior")]
        [Tooltip("If true, this gate can only be completed once.")]
        public bool triggerOnceForThisGate = true;

        [NonSerialized] public bool fired;
    }

    public enum SyringeAnimMode
    {
        Fill,
        Empty
    }

    [Header("Feedback Spawn")]
    public Transform feedbackAnchor;
    public Vector3 feedbackOffset = new Vector3(0f, 0.15f, 0f);

    [Header("Wrong Feedback")]
    public bool showWrongFeedbackWhenNotAtStage = true;
    [Min(0f)] public float wrongFeedbackCooldown = 1.0f;

    [Header("Dependencies (optional)")]
    [SerializeField] private DialogueManager dialogueManager;

    private float lastWrongTime = -999f;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;

        var col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dialogueManager == null) return;
        if (other == null) return;

        if (!other.CompareTag(needleTipTag))
            return;

        // Current stage
        string currentSeg = dialogueManager.GetCurrentSegmentName();
        int currentLine = dialogueManager.GetCurrentLineIndex();

        // Find a matching gate for the current stage
        Gate gate = FindGateForStage(currentSeg, currentLine);

        // If we are at a valid stage for this vial
        if (gate != null)
        {
            if (gate.triggerOnceForThisGate && gate.fired)
                return;

            gate.fired = true;

            if (gate.playSyringeAnimation)
                TryAnimateFromNeedleTipViaSocket(other, gate.animationMode);

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportCorrect(spawnPos);
            ScoreManager.Instance?.RegisterCorrect(gate.targetSegmentName, gate.targetLineIndex);

            dialogueManager.AdvanceDialogue();
            return;
        }

        // Not at a valid stage for this vial -> wrong feedback
        if (showWrongFeedbackWhenNotAtStage && Time.time - lastWrongTime >= wrongFeedbackCooldown)
        {
            lastWrongTime = Time.time;

            Vector3 spawnPos = GetFeedbackSpawnPosition();
            FeedbackManager.Instance?.ReportWrong(spawnPos);

            ScoreManager.Instance?.RegisterMistake(
                dialogueManager.GetCurrentSegmentName(),
                dialogueManager.GetCurrentLineIndex(),
                "Needle entered vial trigger at wrong stage"
            );
        }
    }

    private Gate FindGateForStage(string segmentName, int lineIndex)
    {
        if (gates == null || gates.Count == 0) return null;

        string currentKey = DialogueManager.NormalizeSegmentKey(segmentName);

        for (int i = 0; i < gates.Count; i++)
        {
            Gate g = gates[i];
            if (g == null) continue;

            string gateKey = DialogueManager.NormalizeSegmentKey(g.targetSegmentName);
            if (gateKey == currentKey && g.targetLineIndex == lineIndex)
                return g;
        }

        return null;
    }

    private void TryAnimateFromNeedleTipViaSocket(Collider needleTip, SyringeAnimMode mode)
    {
        if (needleTip == null) return;

        XRGrabInteractable needleGrab = needleTip.GetComponentInParent<XRGrabInteractable>();
        if (needleGrab == null)
            return;

        XRSocketInteractor socket = needleGrab.firstInteractorSelecting as XRSocketInteractor;
        if (socket == null)
            return;

        SyringePlungerAnimator plunger = socket.GetComponentInParent<SyringePlungerAnimator>();
        if (plunger != null)
        {
            if (mode == SyringeAnimMode.Fill) plunger.PlayFill();
            else plunger.PlayEmpty();
            return;
        }

        Animator anim = socket.GetComponentInParent<Animator>();
        if (anim != null)
        {
            if (mode == SyringeAnimMode.Fill) anim.Play("fill", 0, 0f);
            else anim.Play("empty", 0, 0f);
        }
    }

    private Vector3 GetFeedbackSpawnPosition()
    {
        Transform anchor = feedbackAnchor != null ? feedbackAnchor : transform;
        return anchor.position + feedbackOffset;
    }
}
