using UnityEngine;

[CreateAssetMenu(menuName = "Game/Special Items/Special Item Data")]
public class SpecialItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemId;
    public string displayName;
    public SpecialItemType itemType;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Stacking")]
    public bool stackable = false;
    public int maxStackSize = 1;

    [Header("Ownership")]
    public bool ownedByDefault = false;
    public int defaultAmount = 1;

    private void OnValidate()
    {
        if (stackable == false)
            maxStackSize = 1;

        if (maxStackSize < 1)
            maxStackSize = 1;

        if (defaultAmount < 0)
            defaultAmount = 0;

        if (stackable == false && defaultAmount > 1)
            defaultAmount = 1;
    }
}
