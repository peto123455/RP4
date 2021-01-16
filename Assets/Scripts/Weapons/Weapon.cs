using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public string name; // Meno zbrane
    public float cooldownTime, cooldownReload; // Cooldowny
    public int id, slot, maxMagazine, magazine, bulletType, drawSound, sound;
    public int ammo = 0;
    public float cooldown = 0;
    public bool isCooldown = false;
    public bool playerIgnoreCooldown = false;
    public string shootAnimationName, reloadAnimationName;

    public Weapon(int id, int slot, string name, int sound, int drawSound, int bulletType, float cooldownTime, float cooldownReload, int maxMagazine, bool playerIgnoreCooldown, string shootAnimationName, string reloadAnimationName = "") //Konštruktor
    {
        this.id = id;
        this.slot = slot;
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

    public void SetMagazine(int magazine)
    {
        this.magazine = magazine;
    }

}
