using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject blood;
    private int damage = 20;
    private bool critical = false;


    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Enemy")
        {
            Enemy enemy = collision.collider.GetComponent<Enemy>();
            enemy.TakeDamage(damage, critical); ;

            GameObject bloodInstance = Instantiate(blood, gameObject.transform.position, Quaternion.identity);
            Destroy(bloodInstance, 2f);
        }
        else if(collision.collider.tag == "Player")
        {
            Player player = collision.collider.GetComponent<Player>();
            player.TakeDamage(damage/3, critical);

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

}
