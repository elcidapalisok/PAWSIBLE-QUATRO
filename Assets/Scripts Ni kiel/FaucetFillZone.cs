using UnityEngine;

public class FaucetFillZone : MonoBehaviour
{
    [Header("Faucet State")]
    public bool faucetOn = true;

    [Header("Optional: water particle under the faucet")]
    public ParticleSystem waterLeak;

    void OnTriggerEnter(Collider other)
    {
        // If the entering object (or its parent) has ContainerFill, it's a container
        ContainerFill cf = other.GetComponentInParent<ContainerFill>();
        if (cf == null) return;

        cf.SetInFillZone(true);
        cf.SetFaucetOn(faucetOn);
    }

    void OnTriggerExit(Collider other)
    {
        ContainerFill cf = other.GetComponentInParent<ContainerFill>();
        if (cf == null) return;

        cf.SetInFillZone(false);
        cf.SetFaucetOn(false);
    }

    // Call this from your faucet handle button/interaction
    public void SetFaucet(bool on)
    {
        faucetOn = on;

        if (waterLeak != null)
        {
            if (on && !waterLeak.isPlaying) waterLeak.Play();
            if (!on && waterLeak.isPlaying) waterLeak.Stop();
        }
    }
}
