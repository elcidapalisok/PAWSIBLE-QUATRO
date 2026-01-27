using UnityEngine;

[System.Serializable]
public class OrganEntry
{
    public string organName;
    public Sprite organSprite;

    [TextArea(5, 20)]
    public string description;
}
