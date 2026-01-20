using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BinProximityOpener : MonoBehaviour
{
    [SerializeField] private HazardBinController bin;
    [Tooltip("Layers that can open the bin (e.g., Hands, Default, Player). Leave empty to allow all.")]
    public LayerMask allowedLayers;

    private void Awake()
    {
        Collider c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (bin == null) return;

        if (allowedLayers.value != 0)
        {
            int mask = 1 << other.gameObject.layer;
            if ((allowedLayers.value & mask) == 0) return;
        }

        // Open when user/hand enters
        bin.SendMessage("SetOpen", true, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerExit(Collider other)
    {
        if (bin == null) return;

        if (allowedLayers.value != 0)
        {
            int mask = 1 << other.gameObject.layer;
            if ((allowedLayers.value & mask) == 0) return;
        }

        // Close when user/hand exits
        bin.SendMessage("SetOpen", false, SendMessageOptions.DontRequireReceiver);
    }
}
