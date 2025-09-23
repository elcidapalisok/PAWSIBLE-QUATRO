using UnityEngine;

public class ShowNhide : MonoBehaviour
{
    public GameObject target;
public void Show()
{
    GetComponentInChildren<Renderer>().enabled = true;
}

public void Hide()
{
    GetComponentInChildren<Renderer>().enabled = false;
}

}
