using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponList : MonoBehaviour
{

    public class Weapon
    {
        public string name; // Meno zbrane
        public int sound;
        public float cooldownTime, cooldownReload; // Cooldowny
        public int maxMagazine;
        public int magazine;
        public int bulletType;
        public int ammo = 0;
        public float cooldown = 0;
        public bool isCooldown = false;
        public bool hasWeapon = false;
        public bool playerIgnoreCooldown = false;
        public string shootAnimationName, reloadAnimationName;

        public Weapon(string name, int sound, int bulletType, float cooldownTime, float cooldownReload, int maxMagazine, bool playerIgnoreCooldown, string shootAnimationName, string reloadAnimationName) //Konštruktor
        {
            this.name = name;
            this.sound = sound;
            this.bulletType = bulletType;
            this.cooldownTime = cooldownTime;
            this.cooldownReload = cooldownReload;
            this.maxMagazine = maxMagazine;
            this.shootAnimationName = shootAnimationName;
            this.reloadAnimationName = reloadAnimationName;
            this.playerIgnoreCooldown = playerIgnoreCooldown;
        }

        public void Reload() /* Funkcia, ktorá prehodí náboje do zásobníka */
        {
            this.ammo += this.magazine;
            if(this.ammo >= this.maxMagazine)
            {
                this.magazine = this.maxMagazine;
                this.ammo -= this.maxMagazine;
            }
            else
            {
                this.magazine = this.ammo;
                this.ammo = 0;
            }
        }

        public void SetWeapon(bool hasWeapon)
        {
            this.hasWeapon = hasWeapon;
        }

        public bool HasWeapon()
        {
            return this.hasWeapon;
        }

        public void SetCooldown(float cooldown)
        {
            this.isCooldown = true;
            this.cooldown = cooldown;
        }

    }
    
    // Inštancie //
    public Weapon knife = new Weapon("Knife", 1, 0, 0.5f, 0f, 0, false, "Player_knife_attack", "");
    public Weapon glock = new Weapon("Glock-21", 0, 1, 0.15f, 1f, 13, true, "Player_handgun_shoot", "Player_handgun_reload");
    public Weapon ak = new Weapon("AK-47", 3, 2, 0.1f, 1f, 30, false, "Player_rifle_shoot", "Player_rifle_reload");
}
