using UnityEngine;

public class CottonSoakable : MonoBehaviour
{
    public GameObject dryVisual;
    public GameObject soakedVisual;

    private bool isSoaked = false;

    public void Soak()
    {
        if (isSoaked) return;

        isSoaked = true;

        if (dryVisual != null)
            dryVisual.SetActive(false);

        if (soakedVisual != null)
            soakedVisual.SetActive(true);

        Debug.Log("Cotton soaked with disinfectant");
    }

    public bool IsSoaked()
    {
        return isSoaked;
    }
}
