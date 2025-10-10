using UnityEngine;

public class doorDetector : MonoBehaviour
{
    public XRDoor doorScript;
    public string drPawsTag = "Player";
    public bool autoClose = true;
    public float closeDelay = 2f;
    private bool doorOpened = false;

    private Renderer rend;

    private void Start()
    {
        // Optional color indicator (add a simple cube mesh if needed)
        rend = GetComponent<Renderer>();
        if (rend != null) rend.material.color = Color.red;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(drPawsTag))
        {
            if (!doorOpened && doorScript != null)
            {
                Debug.Log("🚪 Door Open Triggered");
                doorScript.ToggleDoor();
                doorOpened = true;
                if (rend != null) rend.material.color = Color.green;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(drPawsTag) && autoClose && doorScript != null)
        {
            Debug.Log("🚪 Door Close Triggered");
            Invoke(nameof(CloseDoor), closeDelay);
            if (rend != null) rend.material.color = Color.red;
        }
    }

    private void CloseDoor()
    {
        if (doorOpened && doorScript != null)
        {
            doorScript.ToggleDoor();
            doorOpened = false;
        }
    }
}
