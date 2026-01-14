using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AnatomyTabManagerXR_Debug : MonoBehaviour
{
    [Header("Tab Roots (MUST be parent objects)")]
    public GameObject skeletalTab;
    public GameObject muscularTab;
    public GameObject visceralTab;

    // ================= BUTTON CALLS =================

    public void OpenSkeletal()
    {
        Debug.Log("BUTTON CLICKED: Skeletal");
        DebugState();

        ShowTab(skeletalTab);
        HideTab(muscularTab);
        HideTab(visceralTab);
    }

    public void OpenMuscular()
    {
        Debug.Log("BUTTON CLICKED: Muscular");
        DebugState();

        ShowTab(muscularTab);
        HideTab(skeletalTab);
        HideTab(visceralTab);
    }

    public void OpenVisceral()
    {
        Debug.Log("BUTTON CLICKED: Visceral");
        DebugState();

        ShowTab(visceralTab);
        HideTab(skeletalTab);
        HideTab(muscularTab);
    }

    // ================= CORE =================

    void HideTab(GameObject tab)
    {
        if (tab == null)
        {
            Debug.LogError("HideTab called with NULL tab");
            return;
        }

        Debug.Log($"HIDE TAB: {tab.name}");

        int rCount = 0, cCount = 0, xrCount = 0;

        foreach (var r in tab.GetComponentsInChildren<Renderer>(true))
        {
            r.enabled = false;
            rCount++;
            Debug.Log($"Renderer OFF → {r.gameObject.name}");
        }

        foreach (var c in tab.GetComponentsInChildren<Collider>(true))
        {
            c.enabled = false;
            cCount++;
            Debug.Log($"Collider OFF → {c.gameObject.name}");
        }

        foreach (var xr in tab.GetComponentsInChildren<XRGrabInteractable>(true))
        {
            xr.enabled = false;
            xrCount++;
            Debug.Log($"XRGrab OFF → {xr.gameObject.name}");
        }

        Debug.Log($"HIDE SUMMARY [{tab.name}] → Renderers:{rCount} Colliders:{cCount} XR:{xrCount}");
    }

    void ShowTab(GameObject tab)
    {
        if (tab == null)
        {
            Debug.LogError("ShowTab called with NULL tab");
            return;
        }

        Debug.Log($"SHOW TAB: {tab.name}");

        int rCount = 0, cCount = 0, xrCount = 0;

        foreach (var r in tab.GetComponentsInChildren<Renderer>(true))
        {
            r.enabled = true;
            rCount++;
            Debug.Log($"Renderer ON → {r.gameObject.name}");
        }

        foreach (var c in tab.GetComponentsInChildren<Collider>(true))
        {
            c.enabled = true;
            cCount++;
            Debug.Log($"Collider ON → {c.gameObject.name}");
        }

        foreach (var xr in tab.GetComponentsInChildren<XRGrabInteractable>(true))
        {
            xr.enabled = true;
            xrCount++;
            Debug.Log($"XRGrab ON → {xr.gameObject.name}");
        }

        Debug.Log($"SHOW SUMMARY [{tab.name}] → Renderers:{rCount} Colliders:{cCount} XR:{xrCount}");
    }

    // ================= DIAGNOSTIC =================

    void DebugState()
    {
        Debug.Log("=== DEBUG STATE CHECK ===");

        CheckTab("Skeletal", skeletalTab);
        CheckTab("Muscular", muscularTab);
        CheckTab("Visceral", visceralTab);

        Debug.Log("=========================");
    }

    void CheckTab(string label, GameObject tab)
    {
        if (tab == null)
        {
            Debug.LogError($"{label} TAB is NULL");
            return;
        }

        int renderers = tab.GetComponentsInChildren<Renderer>(true).Length;
        int colliders = tab.GetComponentsInChildren<Collider>(true).Length;
        int xr = tab.GetComponentsInChildren<XRGrabInteractable>(true).Length;

        Debug.Log($"{label} TAB [{tab.name}] → Renderers:{renderers}, Colliders:{colliders}, XRGrab:{xr}");
    }
}
