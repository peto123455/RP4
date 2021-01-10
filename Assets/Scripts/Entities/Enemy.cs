using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject gunPrefab, floatingText;
    [SerializeField] private int holdingGun = 2;
    [SerializeField] private LayerMask layerMask, collision;
    [SerializeField] private Transform firePoint, firePointO;
    [SerializeField] private GameObject bulletPistol;
    [SerializeField] private bool isDeaf = false;
    //private int health = 100;
    private float fov;
    private float timer = 0;
    private float viewDistance, startingAngle;
    private WeaponList wl = new WeaponList();
    public Sounds sounds;
    private AudioSource sound;
    public HealthSystem healthSystem;

    public static List<Enemy> enemyList = new List<Enemy>();

    public static List<Enemy> GetEnemyList()
    {
        return enemyList;
    }

    private BulletList bulletList = new BulletList();

    void Awake()
    {
        enemyList.Add(this);
        fov = 90f;
        viewDistance = 14f;
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
        GetComponent<Animator>().SetInteger("item", holdingGun);
    }

    private void ChceckVision()
    {
        SetDirection(gameObject.transform.rotation.eulerAngles.z);

        viewDistance = 14f + (GlobalValues.difficulty * 2f);
        int rayCount = 20;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        for (int i = 0; i <= rayCount; i++)
        {
            RaycastHit2D ray = Physics2D.Raycast(gameObject.transform.position, MathFunctions.AngleToVector(angle), viewDistance, layerMask);
            if (ray.collider != null && ray.collider.tag == "Player")
            {
                /*float playerAngle = Mathf.Atan2(ray.collider.transform.position.x - gameObject.transform.position.x, ray.collider.transform.position.y - gameObject.transform.position.y) * Mathf.Rad2Deg;
                gameObject.transform.rotation = Quaternion.Euler(0, 0, -playerAngle);*/

                TurnAtObject(ray.collider.gameObject, true);

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
                CheckSight();
                Shoot(wl.weapons[holdingGun]);
                TakeAmmo(wl.weapons[holdingGun], 1);
                sounds.PlaySound(wl.weapons[holdingGun].sound, sound);
                timer = Time.time + wl.weapons[holdingGun].cooldownTime;
            }
            else ReloadWeapon(wl.weapons[holdingGun]);
        }
    }

    private void CheckSight()
    {
        RaycastHit2D ray = Physics2D.Raycast(firePoint.transform.position, -MathFunctions.AngleToVector(gameObject.transform.rotation.eulerAngles.z - 90f), 1f, collision);
        if(ray.collider != null)
        {
            RaycastHit2D rayO = Physics2D.Raycast(firePointO.transform.position, -MathFunctions.AngleToVector(gameObject.transform.rotation.eulerAngles.z - 90f), 1f, collision);
            Debug.DrawRay(firePointO.transform.position, -MathFunctions.AngleToVector(gameObject.transform.rotation.eulerAngles.z - 90f), Color.green, 1f);
            if(rayO.collider == null) gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, 1f, 0f);
        }
    }

    private void ReloadWeapon(Weapon weapon)
    {
        timer = Time.time + weapon.cooldownReload * 2;
        weapon.magazine = weapon.maxMagazine;
    }

    private void Shoot(Weapon weapon) /* Funkcia, ktorá sa vykoná ak zbraň je nabitá a pripravená k streľbe */
    {
        GameObject bulletPrefab = Instantiate(bulletPistol, firePoint.position, firePoint.rotation);
        BulletList.BulletType bulletType = bulletList.bullets[weapon.bulletType];

        Bullet bullet = bulletPrefab.GetComponent<Bullet>();
        bool critical = bulletType.IsCritical();

        bullet.SetShooter(gameObject);
        bullet.SetBulletDamage(bulletType.Damage(critical), critical);

        Rigidbody2D rbBullet = bulletPrefab.GetComponent<Rigidbody2D>();
        rbBullet.AddForce(firePoint.up * 25f, ForceMode2D.Impulse);
    }

    private void TakeAmmo(Weapon weapon, int amount = 1)
    {
        weapon.magazine -= amount;
        if(weapon.magazine < 0) weapon.magazine = 0;
    }

    public void OnDeath() //Funkcia vykonaná pri smrti
    {
        enemyList.Remove(this);

        GameObject gun = Instantiate(gunPrefab, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
        gun.GetComponent<PickupScript>().SetItem(holdingGun, wl.GetWeaponByID(holdingGun).maxMagazine);
        Destroy(gameObject); //Zničenie objektu nepriateľa
    }

    public void SetDirection(float aimDirection) //Funkcia slúžiaca na nastavenie rotácie FOV
    {
        startingAngle = aimDirection +90f + fov / 2f;
    }

    public void TurnAtObject(GameObject obj, bool overrideDeaf = true)
    {
        if(isDeaf && !overrideDeaf) return;
        float playerAngle = Mathf.Atan2(obj.transform.position.x - gameObject.transform.position.x, obj.transform.position.y - gameObject.transform.position.y) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, -playerAngle);
    }
}
