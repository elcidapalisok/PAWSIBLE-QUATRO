using UnityEngine;

public class IodineWipeTip : MonoBehaviour
{
    [Tooltip("How fast the wound gets tinted while rubbing.")]
    public float cleanRatePerSecond = 0.35f;

    private void OnTriggerStay(Collider other)
    {
        // Look for a wound component on whatever we are touching
        var wound = other.GetComponentInParent<WoundBetadineTint>();
        if (wound == null) return;

        // We are "scrubbing" as long as we remain overlapping
        wound.ApplyBetadine(cleanRatePerSecond * Time.deltaTime);
    }
}
