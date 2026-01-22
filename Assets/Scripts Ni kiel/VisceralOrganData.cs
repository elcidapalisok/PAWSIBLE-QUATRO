using UnityEngine;

[System.Serializable]
public class OrganInfo
{
    public string organName;
    [TextArea(2, 4)]
    public string description;
    [TextArea(2, 4)]
    public string function;
}

public class VisceralOrganData : MonoBehaviour
{
    public OrganInfo[] organs;
}
