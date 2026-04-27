using System;

[Serializable]
public class SpecialItemStack
{
    public SpecialItemData itemData;
    public int amount;

    public SpecialItemStack(SpecialItemData itemData, int amount)
    {
        this.itemData = itemData;
        this.amount = amount;
    }
}
