using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<WeaponData> allWeapons = new();

    private readonly List<WeaponData> ownedWeapons = new();
    private WeaponData equippedWeapon;

    private void Awake()
    {
        InitializeWeapons();
    }

    private void InitializeWeapons()
    {
        ownedWeapons.Clear();
        equippedWeapon = null;

        foreach (var weapon in allWeapons)
        {
            if (weapon == null)
                continue;

            if (weapon.ownedByDefault)
            {
                AddWeapon(weapon);

                if (weapon.equippedByDefault && equippedWeapon == null)
                    equippedWeapon = weapon;
            }
        }

        if (equippedWeapon == null && ownedWeapons.Count > 0)
            equippedWeapon = ownedWeapons[0];
    }

    public void AddWeapon(WeaponData weapon)
    {
        if (weapon == null)
            return;

        if (ownedWeapons.Contains(weapon))
            return;

        ownedWeapons.Add(weapon);
    }

    public bool HasWeapon(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        return ownedWeapons.Contains(weapon);
    }

    public bool EquipWeapon(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        if (HasWeapon(weapon) == false)
            return false;

        equippedWeapon = weapon;
        return true;
    }

    public WeaponData GetEquippedWeapon()
    {
        return equippedWeapon;
    }

    public List<WeaponData> GetOwnedWeapons()
    {
        return ownedWeapons;
    }
}
