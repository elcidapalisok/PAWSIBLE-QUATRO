using UnityEngine;

public class WoundCleanable : MonoBehaviour
{
    public GameObject dirtyWound;
    public GameObject cleanWound;

    private bool isClean = false;

    private void OnTriggerStay(Collider other)
    {
        if (isClean) return;

        CottonSoakable cotton = other.GetComponentInParent<CottonSoakable>();

        if (cotton != null && cotton.IsSoaked())
        {
            isClean = true;

            if (dirtyWound != null)
                dirtyWound.SetActive(false);

            if (cleanWound != null)
                cleanWound.SetActive(true);

            Debug.Log("Wound cleaned");
        }
    }
}
