using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponList
{

    public class Weapon
    {
        public string name; // Meno zbrane
        public int sound;
        public float cooldownTime, cooldownReload; // Cooldowny
        public int id, maxMagazine, magazine, bulletType, drawSound;
        public int ammo = 0;
        public float cooldown = 0;
        public bool isCooldown = false;
        public bool hasWeapon = false;
        public bool playerIgnoreCooldown = false;
        public string shootAnimationName, reloadAnimationName;

        public Weapon(int id, string name, int sound, int drawSound, int bulletType, float cooldownTime, float cooldownReload, int maxMagazine, bool playerIgnoreCooldown, string shootAnimationName, string reloadAnimationName = "") //Konštruktor
        {
            this.id = id;
            this.name = name;
            this.sound = sound;
            this.drawSound = drawSound;
            this.bulletType = bulletType;
            this.maxMagazine = maxMagazine;
            this.cooldownTime = cooldownTime;
            this.cooldownReload = cooldownReload;
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

        public void ReloadByOne() //Slúži na nabíjanie po jednom náboji, určené pre nabíjanie brokovnice
        {
            if(ammo > 0 && magazine < this.maxMagazine)
            {
                this.ammo -= 1;
                this.magazine += 1;
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

        public void GiveAmmo(int amount) /* Funkcia, ktorá po zavolaní dá hráčovi náboje do príslušnej zbrane */
        {
            this.ammo += amount;
        }

        public int GetID()
        {
            return this.id;
        }

        public int GetMagazine()
        {
            return this.magazine;
        }

    }


    public List<Weapon> weapons = new List<Weapon>
    {
        new Weapon(0, "Knife",      1, 8, 0, 0.5f, 0f, 0, false, "Player_knife_attack"),
        new Weapon(1, "Glock-21",   0, 9, 1, 0.2f, 1f, 13, true, "Player_handgun_shoot", "Player_handgun_reload"),
        new Weapon(2, "AK-47",      3, 9, 2, 0.1f, 1f, 30, false, "Player_rifle_shoot", "Player_rifle_reload"),
        new Weapon(3, "Spas-12",    6, 9, 3, 0.7f, 0.65f, 8, true, "Player_shotgun_shoot", "Player_shotgun_reload")
    }; //Tu vytváram zbrane a vkladám ich do zoznamu

    public WeaponList.Weapon GetWeaponByID(int type)
    {
        return weapons[type];
    }
}
