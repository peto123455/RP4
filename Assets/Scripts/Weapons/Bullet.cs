using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject blood;
    private int damage = 20;
    private bool critical = false;
    GameObject shooter;


    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Enemy")
        {
            HealthSystem enemy = collision.collider.GetComponent<HealthSystem>();
            enemy.TakeDamage(damage, critical, shooter);

            if(enemy.gameObject.GetComponent<Turret>() == null)
            {
                GameObject bloodInstance = Instantiate(blood, gameObject.transform.position, Quaternion.identity);
                Destroy(bloodInstance, 2f);
            }
        }
        else if(collision.collider.tag == "Player")
        {
            Player player = collision.collider.GetComponent<Player>();
            player.healthSystem.TakeDamage(damage/(3-GlobalValues.difficulty), critical, shooter);

            GameObject bloodInstance = Instantiate(blood, gameObject.transform.position, Quaternion.identity);
            Destroy(bloodInstance, 2f);
        }
        Destroy(gameObject);
    }

    public void SetBulletDamage(int damage, bool critical)
    {
        this.damage = damage;
        this.critical = critical;
    }

    public GameObject GetShooter()
    {
        return shooter;
    }

    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }
}
