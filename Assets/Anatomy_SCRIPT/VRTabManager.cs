using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Tab
{
    public List<GameObject> items;
}

public class VRTabManager : MonoBehaviour
{
    public Tab[] tabs;
    public Transform content;

    void Start()
    {
        OpenTab(0);
    }

    public void OpenTab(int index)
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        foreach (var prefab in tabs[index].items)
            Instantiate(prefab, content);
    }
}
