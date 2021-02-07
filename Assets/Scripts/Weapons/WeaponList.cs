using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponList
{
    public Weapon primary, secondary, selected, nothing;

    public List<Weapon> weapons = new List<Weapon>
    {
        new Weapon(0, 0, "Knife",      1, 8, 0, 0.5f, 0f, 0, false, "Player_knife_attack"),
        new Weapon(1, 1, "Glock-21",   0, 9, 1, 0.2f, 1f, 13, true, "Player_handgun_shoot", "Player_handgun_reload"),
        new Weapon(2, 2, "AK-47",      3, 9, 2, 0.1f, 1f, 30, false, "Player_rifle_shoot", "Player_rifle_reload"),
        new Weapon(3, 2, "Spas-12",    6, 9, 3, 0.7f, 0.65f, 8, true, "Player_shotgun_shoot", "Player_shotgun_reload")
    }; //Tu vytváram zbrane a vkladám ich do zoznamu

    public Weapon GetWeaponByID(int type)
    {
        return weapons[type];
    }

    public Weapon GetHoldingWeapon()
    {
        return selected;
    }
    public void EquipWeapon(Weapon weapon)
    {
        GetWeaponSlot(weapon) = weapon;
    }

    public void NullWeapon(Weapon weapon)
    {
        GetWeaponSlot(weapon) = null;
    }

    public bool IsWeaponSlotOccupied(Weapon weapon)
    {
        return GetWeaponSlot(weapon) != null;
    }

    public bool HasThisWeapon(Weapon weapon)
    {
        if(weapon.slot == 0) return true;
        return GetWeaponSlot(weapon) == weapon;
    }

    private ref Weapon GetWeaponSlot(Weapon weapon)
    {
        if(weapon.slot == 1) return ref primary;
        else if(weapon.slot == 2) return ref secondary;

        return ref nothing;
    }
}
