﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private int health, maxhealth, armor, maxarmor;

    [SerializeField] GameObject floatingText;

    public void TakeDamage(int damage, bool critical, GameObject shotBy = null) /* Funkcia, ktorá odobere hráčovi životy */
    {
        int tmp;
        if (!critical) tmp = 2;
        else tmp = 1;

        GameObject text = Instantiate(floatingText, transform.position, Quaternion.identity);
        text.GetComponent<FloatingScript>().SetText(damage.ToString(), 0.5f, tmp);

        if(armor > 0)
        {
            armor -= damage;
            if(armor < 0) armor = 0;
        }
        else health -= damage;
        if(health <= 0)
        {
            if(gameObject.GetComponent<Player>() != null) gameObject.GetComponent<Player>().OnDeath();
            else if(gameObject.GetComponent<Enemy>() != null)
            {
                if (shotBy != null && shotBy.tag == "Player") shotBy.GetComponent<Player>().GiveMoney(10); 
                gameObject.GetComponent<Enemy>().OnDeath();
            }
        }
    }
    public void SetHealth(int health)
    {
        this.health = health;
    }

    public void SetMaxHealth(int maxhealth)
    {
        this.maxhealth = maxhealth;
    }

    public int GetMaxHealth()
    {
        return this.maxhealth;
    }

    public void SetMaxShield(int maxarmor)
    {
        this.maxarmor = maxarmor;
    }

    public int GetMaxShield()
    {
        return this.maxarmor;
    }

    public void SetHealthToMax()
    {
        this.health = maxhealth;
    }

    public void SetShieldToMax()
    {
        this.armor = maxarmor;
    }

    public void SetArmor(int armor)
    {
        this.armor = armor;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetArmor()
    {
        return armor;
    }
}