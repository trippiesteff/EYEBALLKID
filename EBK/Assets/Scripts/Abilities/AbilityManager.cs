using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
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

    public List<AbilityData> GetAllAbilities()
    {
        return allAbilities;
    }
}
