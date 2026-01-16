using UnityEngine;

public class AnatomyTabManager_Manual : MonoBehaviour
{
    [Header("Skeletal Objects")]
    public GameObject[] skeletalObjects;

    [Header("Muscular Objects")]
    public GameObject[] muscularObjects;

    [Header("Visceral Objects")]
    public GameObject[] visceralObjects;

    // ================= BUTTON METHODS =================

    public void ShowSkeletal()
    {
        Debug.Log("Show Skeletal");
        SetGroupActive(skeletalObjects, true);
        SetGroupActive(muscularObjects, false);
        SetGroupActive(visceralObjects, false);
    }

    public void ShowMuscular()
    {
        Debug.Log("Show Muscular");
        SetGroupActive(skeletalObjects, false);
        SetGroupActive(muscularObjects, true);
        SetGroupActive(visceralObjects, false);
    }

    public void ShowVisceral()
    {
        Debug.Log("Show Visceral");
        SetGroupActive(skeletalObjects, false);
        SetGroupActive(muscularObjects, false);
        SetGroupActive(visceralObjects, true);
    }

    // ================= CORE =================

    void SetGroupActive(GameObject[] group, bool state)
    {
        if (group == null)
        {
            Debug.LogWarning("Group is NULL");
            return;
        }

        for (int i = 0; i < group.Length; i++)
        {
            if (group[i] == null)
            {
                Debug.LogWarning($"Missing reference at index {i}");
                continue;
            }

            group[i].SetActive(state);
            Debug.Log($"{group[i].name} -> {state}");
        }
    }
}
