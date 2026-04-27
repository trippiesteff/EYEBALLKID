using UnityEngine;

[CreateAssetMenu(menuName = "Game/Tools/Tool Item Data")]
public class ToolItemData : ScriptableObject
{
    [Header("Identity")]
    public string toolId;
    public string displayName;
    public ToolItemType toolType;
    public Sprite icon;

    [Header("Ownership")]
    public bool ownedByDefault;
    public bool equippedByDefault;

    [Header("Use Rules")]
    public bool consumable;
    public int maxStack = 1;
}
