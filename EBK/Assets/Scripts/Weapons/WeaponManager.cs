using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour, IDataPersistence
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

    private WeaponData GetWeaponById(string weaponId)
    {
        foreach (var weapon in allWeapons)
        {
            if (weapon != null && weapon.weaponId == weaponId)
                return weapon;
        }

        return null;
    }

    public void LoadData(GameData data)
    {
        if (data == null)
            return;

        ownedWeapons.Clear();
        equippedWeapon = null;

        foreach (string weaponId in data.ownedWeaponIds)
        {
            WeaponData weapon = GetWeaponById(weaponId);

            if (weapon != null)
            {
                AddWeapon(weapon);
            }
        }

        if (!string.IsNullOrEmpty(data.equippedWeaponId))
        {
            WeaponData weapon = GetWeaponById(data.equippedWeaponId);

            if (weapon != null)
            {
                EquipWeapon(weapon);
            }
        }

        if (equippedWeapon == null && ownedWeapons.Count > 0)
        {
            equippedWeapon = ownedWeapons[0];
        }
    }

    public void SaveData(GameData data)
    {
        data.ownedWeaponIds.Clear();

        foreach (WeaponData weapon in ownedWeapons)
        {
            if (weapon != null)
            {
                data.ownedWeaponIds.Add(weapon.weaponId);
            }
        }

        data.equippedWeaponId = equippedWeapon != null ? equippedWeapon.weaponId : "";
    }
}
