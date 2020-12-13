using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject gunPrefab, floatingText;
    [SerializeField] private int holdingGun = 2;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPistol;
    //private int health = 100;
    private float fov;
    private float timer = 0;
    private float viewDistance, startingAngle;
    private WeaponList wl = new WeaponList();
    public Sounds sounds;
    private AudioSource sound;
    public HealthSystem healthSystem;

    void Awake()
    {
        fov = 90f;
        viewDistance = 13f;
        //wl = GetComponent<WeaponList>();
        //sounds = GetComponent<Sounds>();
        sounds = new Sounds();
        sound = GetComponent<AudioSource>();
        healthSystem = GetComponent<HealthSystem>();
        for(int i = 1; i < GlobalValues.WEAPONS_COUNT; ++i)
        {
            wl.weapons[i].magazine = wl.weapons[i].maxMagazine;
        }
    }

    void Update()
    {
        ChceckVision();
    }

    private void ChceckVision()
    {
        SetDirection(gameObject.transform.rotation.eulerAngles.z);

        int rayCount = 20;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        for (int i = 0; i <= rayCount; i++)
        {
            RaycastHit2D ray = Physics2D.Raycast(gameObject.transform.position, MathFunctions.AngleToVector(angle), viewDistance, layerMask);
            if (ray.collider != null && ray.collider.tag == "Player")
            {
                float playerAngle = Mathf.Atan2(ray.collider.transform.position.x - gameObject.transform.position.x, ray.collider.transform.position.y - gameObject.transform.position.y) * Mathf.Rad2Deg;
                gameObject.transform.rotation = Quaternion.Euler(0, 0, -playerAngle);

                if(Time.time > timer)
                {
                    Attack();
                }
            }
            angle -= angleIncrease;
        }
    }

    private void Attack()
    {
        if (Time.time > timer)
        {
            if(wl.weapons[holdingGun].GetMagazine() > 0)
            {
                Shoot(wl.weapons[holdingGun].bulletType);
                TakeAmmo(wl.weapons[holdingGun], 1);
                sounds.PlaySound(wl.weapons[holdingGun].sound, sound);
                timer = Time.time + wl.weapons[holdingGun].cooldownTime;
            }
            else ReloadWeapon(wl.weapons[holdingGun]);
        }
    }

    private void ReloadWeapon(WeaponList.Weapon weapon)
    {
        timer = Time.time + weapon.cooldownReload * 2;
        weapon.magazine = weapon.maxMagazine;
    }

    private void Shoot(int type) /* Funkcia, ktorá sa vykoná ak zbraň je nabitá a pripravená k streľbe */
    {
        GameObject bullet = Instantiate(bulletPistol, firePoint.position, firePoint.rotation);

        bullet.GetComponent<Bullet>().SetShooter(gameObject);

        switch (type)
        {
            case 1:
                if (Random.Range(0, 10) == 0) bullet.GetComponent<Bullet>().SetBulletDamage(40, true);
                else bullet.GetComponent<Bullet>().SetBulletDamage(20, false);
                break;
            case 2:
                if (Random.Range(0, 5) == 0) bullet.GetComponent<Bullet>().SetBulletDamage(50, true);
                else bullet.GetComponent<Bullet>().SetBulletDamage(25, false);
                break;
            case 3:
                if (Random.Range(0, 2) == 0) bullet.GetComponent<Bullet>().SetBulletDamage(60, true);
                else bullet.GetComponent<Bullet>().SetBulletDamage(40, false);
                break;
        }

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.AddForce(firePoint.up * 25f, ForceMode2D.Impulse);
    }

    private void TakeAmmo(WeaponList.Weapon weapon, int amount = 1)
    {
        weapon.magazine -= amount;
        if(weapon.magazine < 0) weapon.magazine = 0;
    }

    public void OnDeath() //Funkcia vykonaná pri smrti
    {
        GameObject gun = Instantiate(gunPrefab, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
        gun.GetComponent<PickupScript>().SetItem(holdingGun, 13);
        Destroy(gameObject); //Zničenie objektu nepriateľa
    }

    public void SetDirection(float aimDirection) //Funkcia slúžiaca na nastavenie rotácie FOV
    {
        startingAngle = aimDirection +90f + fov / 2f;
    }
}
