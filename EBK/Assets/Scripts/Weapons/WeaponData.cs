using UnityEngine;

[CreateAssetMenu(menuName = "Game/Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Identity")]
    public string weaponId;
    public string displayName;
    public WeaponType weaponType;
    public Sprite icon;

    [Header("Ownership")]
    public bool ownedByDefault;
    public bool equippedByDefault;

    [Header("Damage")]
    public float basePhysicalDamage = 10f;
    public float baseElementalDamage = 0f;
    public ElementType element = ElementType.None;

    [Header("Attack Movement")]
    public Vector2[] attackVelocity;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1f;
    public bool stopMovementAfterAttackLunge = true;

}
