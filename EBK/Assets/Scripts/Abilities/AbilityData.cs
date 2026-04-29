using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Ability Data")]
public class AbilityData : ScriptableObject
{
    public AbilityType abilityType;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public bool unlockedByDefault;
    public int dashCharges = 1;
}