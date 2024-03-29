﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private enum ControlState
    {
        Player,
        Gadget
    }

    /* Deklarácia premenných a objektov */

    [SerializeField] private FOV fov;
    [SerializeField] private Transform firePoint, fireCheckPoint, CPM, CPL, CPR;
    [SerializeField] public GameObject timerIns, itemPrefab;

    /* Public */
    public WeaponList wl = new WeaponList();
    public Gadgets gadgets;
    public Money money = new Money();
    public Animator feet, body;
    public float speed = 5.0f;
    public GameObject bulletPistol, hitEffect, floatingText;
    public int currentLevel;
    public HealthSystem healthSystem;
    public static Player player;

    /* Private */
    private PauseMenu menu;
    private LayerMask enemyLayer, collisionLayer, weaponLayer, enemyVisible;
    private AudioSource sound = new AudioSource();
    private Vector2 mouseVec, mousePos;
    public Sounds sounds;
    private Rigidbody2D rb;
    private GameObject gadget;
    private ControlState controlState = ControlState.Player;
    private bool hasControl;
    private GadgetTimer gadgetTimer;
    private BulletList bulletList = new BulletList();

    void Awake() /* Funkcia, ktorá sa volá pri spustení skriptu */
    {
        /* Layer Masky */
        enemyLayer = LayerMask.GetMask("enemyLayer");
        collisionLayer = LayerMask.GetMask("ObjectNT");
        weaponLayer = LayerMask.GetMask("Weapon");
        enemyVisible = LayerMask.GetMask("enemyLayer", "ObjectNT");

        menu = GameObject.Find("MenuCanvas").GetComponent<PauseMenu>();
        player = this;
        rb = GetComponent<Rigidbody2D>(); /* Zoberie komponent Rigidbody2D a uloží ho do rb*/
        sounds = new Sounds();
        sound = GetComponent<AudioSource>();
        gadgets = GetComponent<Gadgets>();
        healthSystem = GetComponent<HealthSystem>();
        gadgetTimer = GetComponent<GadgetTimer>();
    }

    void Start()
    {
        SaveSystem.OnPlayerLoad();
        SaveSystem.LoadSave();
    }

    void Update() /* Funkcia, ktorá sa vykonáva každý snímok */
    {
        if(!menu.isPaused)
        {
            if(controlState == ControlState.Player) 
            {
                Camera.main.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10);
                /* Rotácia hráča */
                float angle = Mathf.Atan2(mouseVec.x, mouseVec.y) * Mathf.Rad2Deg;
                gameObject.transform.rotation = Quaternion.Euler(0, 0, -angle);
                UpdateMousePos();
                CheckKeys();
                Attack();
                Movement(); //Pohyb hráča
                if(gadget != null) gadget.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            }
            else if (controlState == ControlState.Gadget)
            {
                Camera.main.transform.position = new Vector3(gadget.transform.position.x, gadget.transform.position.y, -10);
                CheckKeysGadget();
                GadgetMovement();
                rb.velocity = new Vector2(0f, 0f);
                feet.SetBool("isWalking", false);
            }
            if (gadgets.equippedGadget != null && gadgets.equippedGadget.ReturnAction() == Gadget.Action.Laser && gadgets.equippedGadget.IsSpawned())
            {
                if(Check()) LaserGadget();
                else LaserRender.Reset();
            }
            UniversalKeys();
        }
    }

    void LateUpdate() /* LateUpdate sa vykonáva až po dokončení všetkých Updatov */
    {
        if(!menu.isPaused)
        {
            UpdateMousePos();
            /* Stará sa o nastavenie pozície a rotácie FOV */
            FOVMovement();
            if(controlState == ControlState.Player) fov.SetDirection((Mathf.Atan2(mouseVec.x, mouseVec.y) * Mathf.Rad2Deg) - 90f);
        }
    }

    private void FixedUpdate() /* Funkcia, ktorá sa pravidelne vykonáva nezávisle od počtu snímkov za sekundu */
    {
        UpdateCooldowns();
    }

    private void CheckKeysGadget()
    {

    }

    private void Attack() /* Funkcia, ktorá sa zavolá pri stlačení ľavého tlačidla myši */
    {
        if (Input.GetButton("Fire1"))
        {
            //Weapon weapon = wl.GetWeaponByID(holdingItem);
            if(!wl.selected.isCooldown && !wl.selected.playerIgnoreCooldown || wl.selected.playerIgnoreCooldown && Input.GetButtonDown("Fire1"))
            {
                if(wl.selected == wl.GetWeaponByID(0))
                {
                    wl.selected.SetCooldown(wl.selected.cooldownTime);
                    body.Play(wl.selected.shootAnimationName);
                    RaycastHit2D checkRay = Physics2D.Raycast(gameObject.transform.position, mousePos - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), 1.5f, enemyLayer);
                    if (checkRay.collider != null && checkRay.collider.tag == "Enemy")
                    {
                        HealthSystem enemy = checkRay.collider.GetComponent<HealthSystem>();
                        enemy.TakeDamage(100, false, gameObject);
                        sounds.PlaySound(wl.selected.sound, sound);
                        if(enemy.gameObject.GetComponent<Turret>() == null)
                        {
                            GameObject hitInstance = Instantiate(hitEffect, checkRay.point, Quaternion.identity);
                            Destroy(hitInstance, 2f);
                        }
                    }
                }
                else if(wl.selected.magazine > 0)
                {
                    Vector2 rayDirection = MathFunctions.InvertVector(mousePos - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));

                    RaycastHit2D checkRay = Physics2D.Raycast(firePoint.transform.position, rayDirection, 1f, collisionLayer); //Slúži na detekciu, či hráč sa nepokúša strielať cez stenu
                    RaycastHit2D checkRay2 = Physics2D.Raycast(fireCheckPoint.transform.position, rayDirection, 1f, collisionLayer); //Dodatočné overenie, ak stojí pri rohu steny
                    //if(checkRay.collider == null)
                    //{
                    wl.selected.SetCooldown(wl.selected.cooldownTime);
                    if(Check()) Shoot(wl.selected); //Podmienku mám iba tu, aby mal hráč pocit že vystrelil
                    wl.selected.magazine -= 1;
                    sounds.PlaySound(wl.selected.sound, sound);
                    body.Play(wl.selected.shootAnimationName);
                    //}
                }
                else if (wl.selected.ammo > 0) ReloadGun(wl.selected);
            }
        }
    }

    private bool Check()
    {
        Vector2 rayDirection = MathFunctions.InvertVector(mousePos - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));

        RaycastHit2D checkRay = Physics2D.Raycast(firePoint.transform.position, rayDirection, 1f, collisionLayer); //Slúži na detekciu, či hráč sa nepokúša strielať cez stenu
        RaycastHit2D checkRay2 = Physics2D.Raycast(fireCheckPoint.transform.position, rayDirection, 1f, collisionLayer); //Dodatočné overenie, ak stojí pri rohu steny

        if(checkRay.collider == null || checkRay2.collider == null) return true;
        return false;
    }

    void Shoot(Weapon weapon) /* Funkcia, ktorá sa vykoná ak zbraň je nabitá a pripravená k streľbe */
    {
        GameObject bulletPrefab = Instantiate(bulletPistol, firePoint.position, firePoint.rotation);
        BulletList.BulletType bulletType = bulletList.bullets[weapon.bulletType];

        Bullet bullet = bulletPrefab.GetComponent<Bullet>();
        bool critical = bulletType.IsCritical();

        bullet.SetShooter(gameObject);
        bullet.SetBulletDamage(bulletType.Damage(critical), critical);

        Rigidbody2D rbBullet = bulletPrefab.GetComponent<Rigidbody2D>();
        rbBullet.AddForce(firePoint.up * 25f, ForceMode2D.Impulse);

        NotifyEnemies();
    }

    private void NotifyEnemies()
    {
        CheckForEnemiesVisible("Enemy");
    }

    private void FOVMovement()
    {
        RaycastHit2D ray = Physics2D.Raycast(CPM.transform.position, mousePos - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), 0.5f, collisionLayer);
        if(ray.collider != null)
        {
            RaycastHit2D rayL = Physics2D.Raycast(CPL.transform.position, mousePos - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), 0.5f, collisionLayer);
            RaycastHit2D rayR = Physics2D.Raycast(CPR.transform.position, mousePos - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), 0.5f, collisionLayer);
            if(rayL.collider != null && rayR.collider != null) fov.SetPosition(transform.position);
            else if(rayL.collider != null) fov.SetPosition(CPR.transform.position);
            else fov.SetPosition(CPL.transform.position);
        }
        else fov.SetPosition(transform.position);
    }

    private void ReloadGun(Weapon weapon) /* Funkcia, ktorá prebije zbraň */
    {

        if (weapon == wl.GetWeaponByID(0)) return;
        else if (weapon.magazine != weapon.maxMagazine && !weapon.isCooldown && weapon.ammo > 0)
        {
            if(weapon == wl.GetWeaponByID(3)) //Nabíjanie brokovnice
            {
                LaserRender.Override(true);
                weapon.SetCooldown(weapon.cooldownReload);
                sounds.PlaySound(5, sound);
                body.Play(weapon.reloadAnimationName);
                weapon.ReloadByOne();
                if(weapon.magazine == weapon.maxMagazine)
                {
                    weapon.SetCooldown(weapon.cooldownReload * 2);
                    StartCoroutine(DelayedSound(weapon.cooldownReload, 7));
                }
                else StartCoroutine(Reload(weapon, weapon.magazine, true));
            }
            else
            {
                LaserRender.Override(true);
                weapon.SetCooldown(weapon.cooldownReload);
                StartCoroutine(Reload(weapon, weapon.magazine, false));
                sounds.PlaySound(2, sound);
                body.Play(weapon.reloadAnimationName);
            }
        }
    }

    IEnumerator DelayedSound(float delay, int sound) //Slúži na prehratie zvuku s oneskorením
    {
        yield return new WaitForSeconds(delay);

        LaserRender.Override(false);
        sounds.PlaySound(sound, this.sound);
    }

    IEnumerator Reload(Weapon weapon, int currentAmmo, bool byOne)
    {
        yield return new WaitForSeconds(0.75f);

        LaserRender.Override(false);

        if(weapon == wl.GetHoldingWeapon() && weapon.magazine == currentAmmo)
        {
            if(byOne) ReloadGun(wl.selected);
            else weapon.Reload();
        }
    }

    private void UpdateCooldowns()
    {
        for(byte i = 0; i < GlobalValues.WEAPONS_COUNT; ++i)
        {
            UpdateCooldown(wl.GetWeaponByID(i));
        }
    }

    private void UpdateCooldown(Weapon weapon)
    {
        if (weapon.isCooldown)
        {
            weapon.cooldown -= Time.deltaTime;

            if (weapon.cooldown <= 0)
            {
                weapon.cooldown = 0;
                weapon.isCooldown = false;
            }
        }
    }

    private void CheckKeys() //Funkcia na kontrolu kláves ak hráč ovláda postavu
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectWeapon(wl.weapons[0]); //Knife
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SelectWeapon(wl.primary);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SelectWeapon(wl.secondary);

        if (Input.GetKeyDown(KeyCode.R) && wl.selected.id != 0) ReloadGun(wl.selected); //Reload
        else if(Input.GetKeyDown(KeyCode.E)) GadgetUse();
        else if(Input.GetKeyDown(KeyCode.Tab)) Pickup();
        else if(Input.GetKeyDown(KeyCode.F)) SwapHand();
    }

    private void SwapHand()
    {
        gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, 1f, 0f);
    }

    private void Pickup()
    {
        GameObject weaponObject = CheckSuround(weaponLayer, "Weapon");
        if(weaponObject != null)
        {
            PickupScript pickup = weaponObject.GetComponent<PickupScript>();

            Weapon weapon = wl.GetWeaponByID(pickup.GetItemType());
            DropWeapon(weapon.slot);
            wl.EquipWeapon(weapon);
            SelectWeapon(weapon);
            if(pickup.GetAmount() > weapon.maxMagazine)
            {
                weapon.SetMagazine(weapon.maxMagazine);
                weapon.GiveAmmo(pickup.GetAmount() - weapon.maxMagazine);
            }
            else weapon.SetMagazine(pickup.GetAmount());

            Destroy(weaponObject);
        }
    }

    private GameObject CheckSuround(LayerMask layer, string tag, int rays = 16, float distance = 1.5f)
    {
        float angle = 0;

        for(int i = 0; i < rays; ++i)
        {
            RaycastHit2D ray = Physics2D.Raycast(gameObject.transform.position, MathFunctions.AngleToVector(angle) * distance, distance, layer);
            angle += 360f/rays;

            if(ray.collider != null && ray.collider.tag == tag)
            {
                return ray.collider.gameObject;
            }
        }

        return null;
    }

    private void CheckForEnemiesVisible(string tag, int rays = 40, float distance = 18f)
    {
        float angle = 0;

        for(int i = 0; i < rays; ++i)
        {
            RaycastHit2D ray = Physics2D.Raycast(gameObject.transform.position, MathFunctions.AngleToVector(angle) * distance, distance, enemyVisible);
            angle += 360f/rays;
            if(ray.collider != null && ray.collider.GetComponent<Enemy>() != null)
            {
                ray.collider.GetComponent<Enemy>().TurnAtObject(gameObject, true);
            }
        }
    }

    private void DropWeapon(int slot)
    {
        GameObject gun = Instantiate(itemPrefab, gameObject.transform.position, Quaternion.identity);

        if(slot == 1)
        {
            gun.GetComponent<PickupScript>().SetItem(wl.primary.id, wl.primary.magazine);
            wl.primary.magazine = 0;
            wl.NullWeapon(wl.primary);
        }
        else if(slot == 2)
        {
            gun.GetComponent<PickupScript>().SetItem(wl.secondary.id, wl.secondary.magazine);
            wl.secondary.magazine = 0;
            wl.NullWeapon(wl.secondary);
        }
    }

    private void UniversalKeys() //Klávesi, ktoé funguje v oboch režimoch ovládania
    {
        if(Input.GetKeyDown(KeyCode.Q)) GadgetControl(); 
    }

    private void GadgetControl()
    {
        if(gadgets.equippedGadget != null && gadgets.equippedGadget.IsSpawned() && gadgets.equippedGadget.ReturnAction() == Gadget.Action.Spawn)
        controlState = controlState == ControlState.Player ? ControlState.Gadget : ControlState.Player;
        else controlState = ControlState.Player;
    }

    private void GadgetUse()
    {
        if(gadgets.equippedGadget != null)
        {
            if (gadgets.equippedGadget.ReturnAction() == Gadget.Action.Spawn)
            {
                if(!gadgets.equippedGadget.IsSpawned())
                {
                    gadget = Instantiate(gadgets.equippedGadget.prefab, gameObject.transform.position, Quaternion.identity);
                    gadgets.equippedGadget.SetSpawned(true);
                    gadgetTimer.StartGadgetTimer(10f);
                }
                else if(Vector2.Distance(gameObject.transform.position, gadget.transform.position) < 2f)
                {
                    gadget.GetComponent<RC>().DestroyRC();
                    gadgets.equippedGadget.SetSpawned(false);
                    gadgetTimer.StopGadgetTimer();
                }
            }
            else if(gadgets.equippedGadget.ReturnAction() == Gadget.Action.Laser)
            {
                gadgets.equippedGadget.SetSpawned(!gadgets.equippedGadget.IsSpawned());
                LaserRender.SetStatus(gadgets.equippedGadget.IsSpawned());
            }
        }
    }

    private void LaserGadget()
    {
        float distance = 20f;
        Vector2 pos2;
        Vector2 rayDirection = MathFunctions.AngleToVector(gameObject.transform.rotation.eulerAngles.z + 90f) * distance;

        RaycastHit2D ray = Physics2D.Raycast(firePoint.transform.position, rayDirection, distance, collisionLayer);
        //Debug.DrawRay(firePoint.transform.position, rayDirection, Color.green, 1f);
        if(ray.collider != null) pos2 = ray.point;
        else
        {
            pos2 = (Vector2) firePoint.position + rayDirection;
        }

        LaserRender.SetPositions((Vector2) firePoint.position, pos2);
    }

    public void SelectWeapon(Weapon weapon) /* Funkcia, ktorá zmení premennú aktuálne držanej veci */
    {
        if(weapon == null || wl.selected == weapon) return;
        body.SetInteger("item", weapon.id); /* Nastaví premennú v Animátorovi, ktorý začne prehrávať príslušnú animáciu, ktorá bola premennej pridelená */
        wl.selected = weapon;
        sounds.PlaySound(weapon.drawSound, sound);
    }

    private void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); 
        float vertical = Input.GetAxisRaw("Vertical");
        
        float currentSpeed = speed;

        if(horizontal != 0f || vertical != 0f)
        {
            if(Input.GetKey(KeyCode.LeftShift)) //Chôdza
            {
                currentSpeed = currentSpeed / 2f;
                feet.speed = 0.4f;
            }
            else //Šprint
            {
                feet.speed = 1f;

                foreach(Enemy enemy in Enemy.GetEnemyList())
                {
                    if(enemy != null && Vector2.Distance(transform.position, enemy.transform.position) < (4.5f + GlobalValues.difficulty))
                    {
                        enemy.TurnAtObject(gameObject, false);
                    }
                }
            }

            feet.SetBool("isWalking", true);
        }
        else feet.SetBool("isWalking", false);

        rb.velocity = new Vector2(horizontal * currentSpeed, vertical * currentSpeed);
    }

    private void GadgetMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        gadget.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontal * speed, vertical * speed);

        float angle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg;
        gadget.transform.rotation = Quaternion.Euler(0, 0, -angle);
    }

    private void UpdateMousePos()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseVec = mousePos - rb.position;
    }

    public void OnDeath()
    {
        menu.ShowDeathMenu();
    }

    public void OnGadgetTimerStop()
    {
        gadget.GetComponent<RC>().DestroyRC();
        gadgets.equippedGadget.SetSpawned(false);
        GadgetControl();
    }
}
