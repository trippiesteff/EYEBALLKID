using UnityEngine;

public class PlayerLoadoutBridge : MonoBehaviour
{
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private ToolSlotManager toolSlotManager;
    [SerializeField] private SpecialItemManager specialItemManager;

    private void Awake()
    {
        if (abilityManager == null)
            abilityManager = GetComponent<AbilityManager>();

        if (weaponManager == null)
            weaponManager = GetComponent<WeaponManager>();

        if (toolSlotManager == null)
            toolSlotManager = GetComponent<ToolSlotManager>();

        if (specialItemManager == null)
            specialItemManager = GetComponent<SpecialItemManager>();
    }

    public bool HasAbility(AbilityType abilityType)
    {
        if (abilityManager == null)
            return false;

        return abilityManager.HasAbility(abilityType);
    }

    public WeaponData GetEquippedWeapon()
    {
        if (weaponManager == null)
            return null;

        return weaponManager.GetEquippedWeapon();
    }

    public ToolItemData GetEquippedTool()
    {
        if (toolSlotManager == null)
            return null;

        return toolSlotManager.GetEquippedTool();
    }

    public bool HasSpecialItem(SpecialItemData item)
    {
        if (specialItemManager == null)
            return false;

        return specialItemManager.HasItem(item);
    }

    public int GetSpecialItemAmount(SpecialItemData item)
    {
        if (specialItemManager == null)
            return 0;

        return specialItemManager.GetOwnedAmount(item);
    }

    public AbilityManager GetAbilityManager() => abilityManager;
    public WeaponManager GetWeaponManager() => weaponManager;
    public ToolSlotManager GetToolSlotManager() => toolSlotManager;
    public SpecialItemManager GetSpecialItemManager() => specialItemManager;
}
