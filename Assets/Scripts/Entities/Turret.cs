using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject bulletPrefab;
    private GameObject head, firePoint;
    private float timer = 0, viewDistance = 10f, startingAngle, fov = 360f;
    private bool isAlive = true;
    private HealthSystem healthSystem;
    private BulletList bulletList = new BulletList();
    public Sounds sounds;
    private AudioSource sound;

    void Awake()
    {
        head = transform.GetChild(0).gameObject;
        firePoint = head.transform.GetChild(0).gameObject;
        sounds = new Sounds();

        /* Komponenty */
        healthSystem = GetComponent<HealthSystem>();
        sound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(isAlive) CheckVision();
    }

    private void CheckVision()
    {
        SetDirection(firePoint.transform.rotation.eulerAngles.z);

        int rayCount = 40;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        for (int i = 0; i <= rayCount; i++)
        {
            RaycastHit2D ray = Physics2D.Raycast(firePoint.transform.position, MathFunctions.AngleToVector(angle), viewDistance, layerMask);
            if (ray.collider != null && ray.collider.tag == "Player")
            {
                float turretAngle = Mathf.Atan2(ray.collider.transform.position.x - firePoint.transform.position.x, ray.collider.transform.position.y - firePoint.transform.position.y) * Mathf.Rad2Deg;
                head.transform.rotation = Quaternion.Euler(0, 0, -turretAngle + 90f);

                if(Time.time > timer)
                {
                    Attack();
                }
            }
            angle -= angleIncrease;
        }
    }
    public void SetDirection(float aimDirection) //Funkcia slúžiaca na nastavenie rotácie FOV
    {
        startingAngle = aimDirection +90f + fov / 2f;
    }

    private void Attack()
    {
        if (Time.time > timer)
        {
            sounds.PlaySound(0, sound);
            Shoot();
            timer = Time.time + 0.15f;
        }
    }

    private void Shoot() /* Funkcia, ktorá sa vykoná ak zbraň je nabitá a pripravená k streľbe */
    {
        GameObject bulletIns = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
        BulletList.BulletType bulletType = bulletList.bullets[1];

        Bullet bullet = bulletIns.GetComponent<Bullet>();
        bool critical = bulletType.IsCritical();

        bullet.SetShooter(gameObject);
        bullet.SetBulletDamage(bulletType.Damage(critical), critical);

        Rigidbody2D rbBullet = bulletIns.GetComponent<Rigidbody2D>();
        rbBullet.AddForce(firePoint.transform.up * 25f, ForceMode2D.Impulse);
    }

    public void OnDeath()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        head.SetActive(false);
        isAlive = false;
    }
}
