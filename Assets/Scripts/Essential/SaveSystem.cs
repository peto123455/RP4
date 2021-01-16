using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    static HealthSystem healthSystem;
    static Money money;
    static WeaponList wl;

    public static void OnPlayerLoad()
    {
        healthSystem = Player.player.healthSystem;
        money = Player.player.money;
        wl = Player.player.wl;
    }

    public static void LoadSave()
    {
        healthSystem.SetHealth(PlayerPrefs.GetInt("health", 100));
        healthSystem.SetArmor(PlayerPrefs.GetInt("armor", 100));
        healthSystem.SetMaxHealth(100 + PlayerPrefs.GetInt("healthLvl", 0) * 5);
        healthSystem.SetMaxShield(100 + PlayerPrefs.GetInt("shieldLvl", 0) * 5);
        GlobalValues.fov = (byte) PlayerPrefs.GetInt("fovLvl", 0);
        GlobalValues.difficulty = (byte) PlayerPrefs.GetInt("difficulty", 0);
        money.SetMoney(PlayerPrefs.GetInt("money", 0));

        if(PlayerPrefs.GetInt("primary", 0) != 0) wl.primary = wl.GetWeaponByID(PlayerPrefs.GetInt("primary", 0));
        if(PlayerPrefs.GetInt("secondary", 0) != 0) wl.secondary = wl.GetWeaponByID(PlayerPrefs.GetInt("secondary", 0));
        Weapon lastSelect = wl.GetWeaponByID(PlayerPrefs.GetInt("selected", 0));

        for(int i = 1; i < GlobalValues.WEAPONS_COUNT; ++i)
        {
            LoadWeapon(wl.GetWeaponByID(i));
        }

        Player.player.currentLevel = PlayerPrefs.GetInt("level", 1);
        Player.player.SelectWeapon(lastSelect);
    }

    private static void LoadWeapon(Weapon weapon)
    {
        weapon.ammo = PlayerPrefs.GetInt("ammo" + weapon.id, 0);
        weapon.magazine = PlayerPrefs.GetInt("magazine" + weapon.id, 0);
        //weapon.SetWeapon(MathFunctions.IntToBool(PlayerPrefs.GetInt("has" + weapon.id, 0)));
    }

    public static void SaveGame()
    {
        PlayerPrefs.SetInt("health", healthSystem.GetHealth());
        PlayerPrefs.SetInt("armor", healthSystem.GetArmor());
        PlayerPrefs.SetInt("money", money.GetMoney());

        if(wl.primary != null) PlayerPrefs.SetInt("primary", wl.primary.id);
        else PlayerPrefs.SetInt("primary", 0);
        if(wl.secondary != null) PlayerPrefs.SetInt("secondary", wl.secondary.id);
        else PlayerPrefs.SetInt("secondary", 0);
        if(wl.selected != null) PlayerPrefs.SetInt("selected", wl.selected.id);
        else PlayerPrefs.SetInt("selected", 0);

        for(int i = 1; i < GlobalValues.WEAPONS_COUNT; ++i)
        {
            SaveWeapon(wl.GetWeaponByID(i));
        }
    }

    private static void SaveWeapon(Weapon weapon)
    {
        PlayerPrefs.SetInt("ammo" + weapon.id, weapon.ammo);
        PlayerPrefs.SetInt("magazine" + weapon.id, weapon.magazine);
        //PlayerPrefs.SetInt("has" + weapon.id, MathFunctions.BoolToInt(weapon.HasWeapon()));
    }

    public static void CreateGame() // Nastaví zákl. hodnoty pre novú hru.
    {
        WeaponList weaponTemp = new WeaponList();
        /* Základné */
        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.SetInt("health", 100);
        PlayerPrefs.SetInt("armor", 0);
        PlayerPrefs.SetInt("money", 0);
        PlayerPrefs.SetInt("primary", 0);
        PlayerPrefs.SetInt("secondary", 0);
        PlayerPrefs.SetInt("selected", 0);
        PlayerPrefs.SetInt("healthLvl", 0);
        PlayerPrefs.SetInt("shieldLvl", 0);
        PlayerPrefs.SetInt("fovLvl", 0);
        for(int i = 1; i < GlobalValues.WEAPONS_COUNT; ++i)
        {
            CreateWeaponSave(weaponTemp.GetWeaponByID(i));
        }
    }

    private static void CreateWeaponSave(Weapon weapon)
    {
        PlayerPrefs.SetInt("ammo" + weapon.id, 0);
        PlayerPrefs.SetInt("magazine" + weapon.id, 0);
        PlayerPrefs.SetInt("has" + weapon.id, 0);
    }
}
