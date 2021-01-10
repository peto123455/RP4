using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletList
{
    public class BulletType
    {
        private int id, critChance, damage, damageCritical;
        private bool hasCritical;

        public BulletType(int id, int critChance, int damage, bool hasCritical, int damageCritical = 0)
        {
            this.id = id;
            this.critChance = critChance;
            this.damage = damage;
            this.hasCritical = hasCritical;
            this.damageCritical = damageCritical;
        }

        public int Damage(bool critic = false)
        {
            if(critic) return this.damageCritical;
            return this.damage;
        }

        public bool IsCritical()
        {
            if(this.hasCritical && Random.Range(0, this.critChance) == 0) return true;
            return false;
        }

        public int GetID()
        {
            return this.id;
        }
    }

    public List<BulletType> bullets = new List<BulletType>
    {
        new BulletType(0, 0, 0, false),
        new BulletType(1, 10, 20, true, 40),
        new BulletType(2, 5, 25, true, 50),
        new BulletType(3, 2, 40, true, 60)
    };
}