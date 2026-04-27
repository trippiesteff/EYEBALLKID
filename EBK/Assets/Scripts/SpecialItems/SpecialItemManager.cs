using System.Collections.Generic;
using UnityEngine;

public class SpecialItemManager : MonoBehaviour
{
    [SerializeField] private List<SpecialItemData> allSpecialItems = new();
    [SerializeField] private List<SpecialItemStack> ownedSpecialItems = new();

    private void Awake()
    {
        InitializeDefaultItems();
    }

    private void InitializeDefaultItems()
    {
        ownedSpecialItems.Clear();

        foreach (var item in allSpecialItems)
        {
            if (item == null)
                continue;

            if (item.ownedByDefault == false)
                continue;

            AddItem(item, item.defaultAmount);
        }
    }

    public bool HasItem(SpecialItemData item)
    {
        return GetOwnedAmount(item) > 0;
    }

    public int GetOwnedAmount(SpecialItemData item)
    {
        if (item == null)
            return 0;

        SpecialItemStack stack = FindStack(item);
        return stack != null ? stack.amount : 0;
    }

    public bool AddItem(SpecialItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        SpecialItemStack stack = FindStack(item);

        if (stack == null)
        {
            int startingAmount = item.stackable ? Mathf.Min(amount, item.maxStackSize) : 1;
            ownedSpecialItems.Add(new SpecialItemStack(item, startingAmount));
            return true;
        }

        if (item.stackable == false)
        {
            stack.amount = Mathf.Max(stack.amount, 1);
            return true;
        }

        stack.amount = Mathf.Clamp(stack.amount + amount, 0, item.maxStackSize);
        return true;
    }

    public bool RemoveItem(SpecialItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        SpecialItemStack stack = FindStack(item);

        if (stack == null)
            return false;

        stack.amount -= amount;

        if (stack.amount <= 0)
            ownedSpecialItems.Remove(stack);

        return true;
    }

    public bool ConsumeItem(SpecialItemData item, int amount = 1)
    {
        if (HasEnough(item, amount) == false)
            return false;

        return RemoveItem(item, amount);
    }

    public bool HasEnough(SpecialItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return false;

        return GetOwnedAmount(item) >= amount;
    }

    public List<SpecialItemStack> GetOwnedSpecialItems()
    {
        return ownedSpecialItems;
    }

    private SpecialItemStack FindStack(SpecialItemData item)
    {
        return ownedSpecialItems.Find(x => x.itemData == item);
    }
}
