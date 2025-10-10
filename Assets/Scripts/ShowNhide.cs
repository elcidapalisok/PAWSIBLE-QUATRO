using UnityEngine;

public class ShowNhide : MonoBehaviour
{
    public GameObject target;

    public void Show()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }
    }

    public void Hide()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
    }
}
