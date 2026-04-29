using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour, IDataPersistence
{
    [Header("All known abilities")]
    [SerializeField] private List<AbilityData> allAbilities = new();

    private readonly HashSet<AbilityType> unlockedAbilities = new();

    private void Awake()
    {
        InitializeDefaultAbilities();
    }

    private void InitializeDefaultAbilities()
    {
        unlockedAbilities.Clear();

        foreach (var ability in allAbilities)
        {
            if (ability != null && ability.unlockedByDefault)
                unlockedAbilities.Add(ability.abilityType);
        }
    }

    public bool HasAbility(AbilityType abilityType)
    {
        return unlockedAbilities.Contains(abilityType);
    }

    public AbilityData GetAbilityData(AbilityType abilityType)
    {
        foreach (var ability in allAbilities)
        {
            if (ability != null && ability.abilityType == abilityType)
                return ability;
        }
        return null;
    }

    public int GetDashCharges()
    {
        AbilityData dashData = GetAbilityData(AbilityType.Dash);
        if (dashData != null)
            return dashData.dashCharges;
        return 1;
    }

    public void UnlockAbility(AbilityType abilityType)
    {
        if (abilityType == AbilityType.None)
            return;

        unlockedAbilities.Add(abilityType);
    }

    public void LockAbility(AbilityType abilityType)
    {
        if (abilityType == AbilityType.None)
            return;

        unlockedAbilities.Remove(abilityType);
    }

    public void UnlockAbility(AbilityData abilityData)
    {
        if (abilityData == null)
            return;

        UnlockAbility(abilityData.abilityType);
    }

    public void SwapAbility(AbilityData oldAbility, AbilityData newAbility)
    {
        if (oldAbility != null)
        {
            allAbilities.Remove(oldAbility);
            unlockedAbilities.Remove(oldAbility.abilityType);
        }

        if (newAbility != null)
        {
            allAbilities.Add(newAbility);
            unlockedAbilities.Add(newAbility.abilityType);
        }
    }

    public List<AbilityData> GetAllAbilities()
    {
        return allAbilities;
    }

    public void LoadData(GameData data)
    {
        if (data == null)
            return;

        if (data.unlockedAbilityIds == null || data.unlockedAbilityIds.Count == 0)
            return;

        unlockedAbilities.Clear();

        foreach (string abilityId in data.unlockedAbilityIds)
        {
            if (System.Enum.TryParse(abilityId, out AbilityType abilityType))
            {
                if (abilityType != AbilityType.None)
                {
                    unlockedAbilities.Add(abilityType);
                }
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.unlockedAbilityIds.Clear();

        foreach (AbilityType abilityType in unlockedAbilities)
        {
            if (abilityType != AbilityType.None)
            {
                data.unlockedAbilityIds.Add(abilityType.ToString());
            }
        }
    }
}