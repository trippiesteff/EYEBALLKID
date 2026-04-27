using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO defaulStatSetup;
    // public ElementType elementType;
    public Stat_ResourceGroup resources;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;
    public Stat_MajorGroup major;

    public float GetElementalDamage(out ElementType element, float scaleFactor = 1f)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();
        float bonusElementalDamage = major.intelligence.GetValue(); // Bonus elemental damage from intelligence +1 per INT

        float highestDamage = fireDamage;
        element = ElementType.Fire;
        if(iceDamage > highestDamage)
            {
                highestDamage = iceDamage;
                element = ElementType.Ice;
            }
        if(lightningDamage > highestDamage)
            {
                highestDamage = lightningDamage;
                element = ElementType.Lightning;
            }

        if(highestDamage <= 0)
        {
            element = ElementType.None;
            return 0;
        }

        float bonusFire = (element == ElementType.Fire) ? 0 : fireDamage * 0.5f; // Bonus damage for non-highest elements (50% of their value)
        float bonusIce = (element == ElementType.Ice) ? 0 : iceDamage * 0.5f;
        float bonusLightning= (element == ElementType.Lightning) ? 0 : lightningDamage * 0.5f;

        float weakerElementsDamage = bonusFire + bonusIce + bonusLightning;


        float finalDamage = highestDamage + weakerElementsDamage + bonusElementalDamage;

        return finalDamage* scaleFactor;    
    }

    public float GetElementalResistance(ElementType element)
    {
       float baseResistance = 0;
       float bonusResistance = major.intelligence.GetValue() * 0.5f; // Bonus elemental resistance from intelligence +0,5 per INT

       switch(element)
       {
        case ElementType.Fire:
            baseResistance = defense.fireRes.GetValue();
            break;
        case ElementType.Ice:
            baseResistance = defense.iceRes.GetValue();
            break;
        case ElementType.Lightning:
            baseResistance = defense.lightningRes.GetValue();
            break;
       }
       float resistance = baseResistance + bonusResistance;
       float resistanceCap = 75f; // Resistance will be capped at 75%
       float finalResistance = Mathf.Clamp(resistance,0,resistanceCap)/100; // convert to multiplier (e.g. 50 / 100 = 0,5f - multiplier)
       return finalResistance;

    }

    public float GetPhysicalDamage(out bool isCrit, float scaleFactor = 1f)
    {
        float baseDamage = offense.damage.GetValue();
        float bonusDamage = major.strength.GetValue();
        float totalBaseDamage = baseDamage + bonusDamage;

        float baseCritChance = offense.critChance.GetValue();
        float bonusCritChance = major.agility.GetValue() * 3f; // bonus crit chance from agility +0,3% per AGI
        float critChance = baseCritChance + bonusCritChance;

        float baseCritPower = offense.critPower.GetValue();
        float bonusCritPower = major.strength.GetValue() * .5f; // bonus crit chance from strength: +0,5% per STR
        float critPower = (baseCritPower + bonusCritPower) / 100; // Total Crit Power as multiplier (e.g. 150 / 100 = 1.5f - multiplier)

        isCrit = Random.Range(0,100) < critChance;
        float finalDamage = isCrit ? totalBaseDamage * critPower : totalBaseDamage;

        return finalDamage * scaleFactor;
    }

    public float GetArmorMitigation(float armorReduction)
    {
        float baseArmor = defense.armor.GetValue();
        float bonusArmor = major.vitality.GetValue(); // Bonus armor from vitality +1 per VIT;
        float totalArmor = baseArmor + bonusArmor;

        float reductionMultiplier = Mathf.Clamp(1 - armorReduction,0,1); 
        float effectiveArmor = totalArmor * reductionMultiplier;

        float mitigation = effectiveArmor /(effectiveArmor + 100);
        float mitigationCap = .85f; // max mitigation will be capped at 85%

        float finalMitigation = Mathf.Clamp(mitigation,0,mitigationCap);

        return finalMitigation;
    }

    public float GetArmorReduction()
    {
        float finalReduction = offense.armorReduction.GetValue() / 100;
        return finalReduction;
    }
    

    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue()*.5f; // each agility point gives you 0,5% of evasion;

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 85f;

        float finalEvasion = Mathf.Clamp(totalEvasion,0,evasionCap);

        return finalEvasion;
    
    }
    public float GetMaxHealth()
    {
        float baseMaxHealth = resources.maxHealth.GetValue();
        float bonusMaxHealth = major.vitality.GetValue() * 5;
        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;
        
        return finalMaxHealth;
    }

    public Stat GetStatByType(StatType type)
    {
        switch(type)
        {
            case StatType.MaxHealth:
                return resources.maxHealth;
            case StatType.HealthRegen:
                return resources.healthRegen;
                
            case StatType.Strength:
                return major.strength;
            case StatType.Agility:
                return major.agility;
            case StatType.Intelligence:
                return major.intelligence;
            case StatType.Vitality:
                return major.vitality;

            case StatType.AttackSpeed:
                return offense.attackSpeed;
            case StatType.Damage:
                return offense.damage;
            case StatType.CritChance:
                return offense.critChance;
            case StatType.CritPower:
                return offense.critPower;
            case StatType.ArmorReduction:
                return offense.armorReduction;

            case StatType.FireDamage:
                return offense.fireDamage;
            case StatType.IceDamage:
                return offense.iceDamage;
            case StatType.LightningDamage:
                return offense.lightningDamage;
                
            case StatType.Armor:
                return defense.armor;
            case StatType.Evasion:
                return defense.evasion;

            case StatType.IceResistance:
                return defense.iceRes;
            case StatType.FireResistance:
                return defense.fireRes;
            case StatType.LightningResistance:
                return defense.lightningRes;

            default:
                Debug.LogError("Stat type " + type + " not found!");
                return null;

        }
    }

    [ContextMenu("Update Default Stat Setup")]
    public void ApplyDefaultStatSetup()
    {
        if(defaulStatSetup == null)
        {
            Debug.Log("No default stat setup assigned ");
            return;
        }

        resources.maxHealth.SetBaseValue(defaulStatSetup.maxHealth);
        resources.healthRegen.SetBaseValue(defaulStatSetup.healthRegen);


        major.strength.SetBaseValue(defaulStatSetup.strength);
        major.agility.SetBaseValue(defaulStatSetup.agility);
        major.intelligence.SetBaseValue(defaulStatSetup.intelligence);
        major.vitality.SetBaseValue(defaulStatSetup.vitality);

        offense.attackSpeed.SetBaseValue(defaulStatSetup.attackSpeed);
        offense.damage.SetBaseValue(defaulStatSetup.damage);
        offense.critChance.SetBaseValue(defaulStatSetup.critChance);
        offense.critPower.SetBaseValue(defaulStatSetup.critPower);
        offense.armorReduction.SetBaseValue(defaulStatSetup.armorReduction);

        offense. iceDamage.SetBaseValue(defaulStatSetup.iceDamage);
        offense. fireDamage.SetBaseValue(defaulStatSetup.fireDamage);
        offense. lightningDamage.SetBaseValue(defaulStatSetup.lightningDamage);

        defense.armor.SetBaseValue(defaulStatSetup.armor);
        defense.evasion.SetBaseValue(defaulStatSetup.evasion);

        defense.iceRes.SetBaseValue(defaulStatSetup.iceResistance);
        defense.fireRes.SetBaseValue(defaulStatSetup.fireResistance);
        defense.lightningRes.SetBaseValue(defaulStatSetup.lightningResistance); 
    }



}
