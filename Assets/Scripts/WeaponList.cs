using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponList : MonoBehaviour
{

    public class Weapon
    {
        public string name; // Meno zbrane
        public float cooldownTime, cooldownReload; // Cooldowny
        public int maxMagazine;
        public int magazine;
        public int ammo = 0;
        public float cooldown = 0;
        public bool isCooldown = false;
        public bool hasWeapon = false;

        public Weapon(string name, float cooldownTime, float cooldownReload, int maxMagazine) //Konštruktor
        {
            this.name = name;
            this.cooldownTime = cooldownTime;
            this.cooldownReload = cooldownReload;
            this.maxMagazine = maxMagazine;
        }

        public void SetWeapon(bool hasWeapon)
        {
            this.hasWeapon = hasWeapon;
        }

        public bool HasWeapon()
        {
            return this.hasWeapon;
        }

    }
    
    // Inštancie //
    public Weapon knife = new Weapon("Knife", 0.5f, 0f, 0);
    public Weapon glock = new Weapon("Glock-21", 0.15f, 1f, 13);
    public Weapon ak = new Weapon("AK-47", 0.1f, 1f, 30);
}
