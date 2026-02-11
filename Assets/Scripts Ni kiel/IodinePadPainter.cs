using UnityEngine;


public class IodinePadPainter : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    public Transform paintTip;
    public LayerMask woundLayer;

    [Header("Rubbing")]
    public float minSpeedToPaint = 0.05f;
    public float paintInterval = 0.02f;
    public float rayDistance = 0.05f;

    Vector3 _lastTipPos;
    float _timer;

    void Awake()
    {
        if (grab == null) grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (paintTip != null) _lastTipPos = paintTip.position;
    }

    void Update()
    {
        if (grab == null || paintTip == null) return;

        // Only paint when held
        if (!grab.isSelected)
        {
            _lastTipPos = paintTip.position;
            _timer = 0;
            return;
        }

        float speed = Vector3.Distance(paintTip.position, _lastTipPos) / Mathf.Max(Time.deltaTime, 0.0001f);
        _lastTipPos = paintTip.position;
        if (speed < minSpeedToPaint) return;

        _timer += Time.deltaTime;
        if (_timer < paintInterval) return;
        _timer = 0;

        Ray ray = new Ray(paintTip.position, -paintTip.up);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, woundLayer, QueryTriggerInteraction.Ignore))
        {
            var receiver = hit.collider.GetComponentInParent<WoundStainReceiver>();
            if (receiver != null) receiver.PaintAtUV(hit.textureCoord);
        }
    }
}
