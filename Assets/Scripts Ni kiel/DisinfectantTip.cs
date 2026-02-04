using UnityEngine;

public class DisinfectantTip : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Look for CottonSoakable on the object that entered (or its parent)
        CottonSoakable soakable = other.GetComponentInParent<CottonSoakable>();

        if (soakable != null)
        {
            soakable.Soak();
        }
    }
}
