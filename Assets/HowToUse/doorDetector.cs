using UnityEngine;
using System.Collections;

public class doorDetector : MonoBehaviour
{
    public XRDoor doorScript;
    public string drPawsTag = "Player";
    public bool autoClose = true;
    public float closeDelay = 2f;
    private bool doorOpened = false;

    public DrPawsPathWalker_CutScene drPawsPathWalker;

    private Renderer rend;

    private void Start()
    {
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

                // ✅ Trigger the door opening animation
                StartCoroutine(PlayDoorAnimation());

                doorOpened = true;
                if (rend != null) rend.material.color = Color.green;
            }
        }
    }

    private IEnumerator PlayDoorAnimation()
    {
        drPawsPathWalker.OpeningDoor();
        Debug.Log("🎬 Door opening animation started!");

        // Wait for a short time (adjust as needed)
        yield return new WaitForSeconds(1f);

        // Stop the animation after the delay
        drPawsPathWalker.animator?.SetBool("isOpeningDoor", false);
        Debug.Log("⏹️ Door opening animation stopped!");
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
