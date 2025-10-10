using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WearGloves : MonoBehaviour
{
    public Renderer leftHand;
    public Renderer rightHand;
    public Material gloveMaterial;
    public Material defaultMaterial;

    private bool isWearingGloves = false;

    public void OnGloveGrab()
    {
        if (isWearingGloves)
        {
            leftHand.material = defaultMaterial;
            rightHand.material = defaultMaterial;
        }
        else
        {
            leftHand.material = gloveMaterial;
            rightHand.material = gloveMaterial;
        }

        isWearingGloves = !isWearingGloves;
    }
}
