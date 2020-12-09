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
    private int health = 100;
    private float fov;
    private float timer = 0;
    private float viewDistance, startingAngle;
    private WeaponList wl;
    private Sounds sounds;
    private AudioSource sound;

    void Awake()
    {
        fov = 90f;
        viewDistance = 13f;
        wl = GetComponent<WeaponList>();
        sounds = GetComponent<Sounds>();
        sound = GetComponent<AudioSource>();
        wl.weapons[1].ammo = wl.weapons[1].maxMagazine;
        wl.weapons[2].ammo = wl.weapons[2].maxMagazine;
        wl.weapons[3].ammo = wl.weapons[3].maxMagazine;
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
            RaycastHit2D ray = Physics2D.Raycast(gameObject.transform.position, AngleToVector(angle), viewDistance, layerMask);
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
        /* GLOCK 21 */
        if (holdingGun == 1 && Time.time > timer)
        {
            if(CheckAmmo(wl.weapons[1]))
            {
                Shoot(1);
                sounds.PlaySound(0, sound);
                timer = Time.time + wl.weapons[1].cooldownTime;
            }
            else ReloadWeapon(wl.weapons[1]);
        }
        /* AK-47 */
        else if (holdingGun == 2 && Time.time > timer)
        {
            if(CheckAmmo(wl.weapons[2]))
            {
                Shoot(2);
                sounds.PlaySound(3, sound);
                timer = Time.time + wl.weapons[2].cooldownTime;
            }
            else ReloadWeapon(wl.weapons[2]);
        }
    }

    private bool CheckAmmo(WeaponList.Weapon weapon)
    {
        return (weapon.ammo > 0);
    }

    private void ReloadWeapon(WeaponList.Weapon weapon)
    {
        timer = Time.time + weapon.cooldownReload;
        weapon.ammo = weapon.maxMagazine;
    }

    private void Shoot(int type) /* Funkcia, ktorá sa vykoná ak zbraň je nabitá a pripravená k streľbe */
    {
        GameObject bullet = Instantiate(bulletPistol, firePoint.position, firePoint.rotation);

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
        weapon.ammo -= amount;
        if(weapon.ammo < 0) weapon.ammo = 0;
    }

    public void TakeDamage(int damage, bool critical) //Funkcia, ktorá po zavolaní poškodí nepriateľa
    {
        int tmp; //Pomocná premenná na zistenie, či bolo poškodenie kritické
        if (!critical) tmp = 2; //Žltá
        else tmp = 1; //Červená

        health -= damage; //Zoberie hráčovi životy
        GameObject text = Instantiate(floatingText, transform.position, Quaternion.identity); //Vytvorí text v hre
        text.GetComponent<FloatingScript>().SetText(damage.ToString(), 0.5f, tmp); //A nastaví text na damage
        if (health <= 0) OnDeath(); //Ak má nepriateľ životy menej alebo rovné 0 vykoná sa funkcia OnDeath()
    }

    private void OnDeath() //Funkcia vykonaná pri smrti
    {
        GameObject gun = Instantiate(gunPrefab, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
        gun.GetComponent<PickupScript>().SetItem(holdingGun, 13);
        Destroy(gameObject); //Zničenie objektu nepriateľa
    }

    private Vector3 AngleToVector(float angle) //Funkcia, ktorá prepočíta Uhol na Vektor
    {
        float angleRad = angle * (Mathf.PI / 180f); //Prevenie stupne na radiany
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)); 
    }

    private float VectorToAngle(Vector3 direction)
    {
        direction.Normalize();
        float n = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    private float SimplifyAngle(float angle)
    {
        while(angle < 0 || angle > 360)
        {
            if(angle < 0) angle += 360;
            else angle -= 360;
        }

        return angle;
    }

    public void SetDirection(float aimDirection) //Funkcia slúžiaca na nastavenie rotácie FOV
    {
        startingAngle = aimDirection +90f + fov / 2f;
    }
}
