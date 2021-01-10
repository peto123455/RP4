using System.Collections;
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
    [SerializeField] private PauseMenu menu;
    [SerializeField] private GameObject timerIns, itemPrefab;

    public Animator feet, body;
    public float speed = 5.0f;
    public GameObject bulletPistol, hitEffect, floatingText;
    public int currentLevel, money;
    public LayerMask enemyLayer, collisionLayer, weaponLayer;
    private AudioSource sound = new AudioSource();

    private float speedAnimation;

    //private int holdingItem = 0;
    private Vector2 mouseVec, mousePos;
    public Sounds sounds;
    private Rigidbody2D rb;
    public WeaponList wl = new WeaponList();
    private Gadgets gadgets;
    private GameObject gadget;
    private ControlState controlState = ControlState.Player;

    public HealthSystem healthSystem;

    private bool hasControl;

    private class GadgetTimer
    {
        public float time, timeLeft;
        public bool active;

        public GadgetTimer(float time, bool active)
        {
            this.time = time;
            this.timeLeft = time;
            this.active = active;
        }
    }
    private GadgetTimer gadgetTimer = new GadgetTimer(0f, false);

    private BulletList bulletList = new BulletList();

    void Start() /* Funkcia, ktorá sa volá pri spustení skriptu */
    {
        rb = GetComponent<Rigidbody2D>(); /* Zoberie komponent Rigidbody2D a uloží ho do rb*/
        sounds = new Sounds();
        sound = GetComponent<AudioSource>();
        gadgets = GetComponent<Gadgets>();
        healthSystem = GetComponent<HealthSystem>();

        gadgets.SetGadget(gadgets.rcCar, true);
        gadgets.EquipGadget(gadgets.rcCar);

        LoadSave();
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
                feet.SetFloat("Speed", 0);
            }

            UniversalKeys();
            UpdateGadgetTimer();
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
        /*if(controlState == ControlState.Player)
        {
            Movement(); //Pohyb hráča
            if(gadget != null) gadget.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        }
        else if(controlState == ControlState.Gadget)
        {
            GadgetMovement();
            rb.velocity = new Vector2(0f, 0f);
            feet.SetFloat("Speed", 0);
        }*/
        UpdateCooldowns();
        CheckPlayerStatus();
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
                        Enemy enemy = checkRay.collider.GetComponent<Enemy>();
                        enemy.healthSystem.TakeDamage(100, false, gameObject);
                        sounds.PlaySound(wl.selected.sound, sound);
                        GameObject hitInstance = Instantiate(hitEffect, checkRay.point, Quaternion.identity);
                        Destroy(hitInstance, 2f);
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
                    if(checkRay.collider == null || checkRay2.collider == null) Shoot(wl.selected); //Podmienku mám iba tu, aby mal hráč pocit že vystrelil
                    wl.selected.magazine -= 1;
                    sounds.PlaySound(wl.selected.sound, sound);
                    body.Play(wl.selected.shootAnimationName);
                    //}
                }
                else if (wl.selected.ammo > 0) ReloadGun(wl.selected);
            }
        }
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

        sounds.PlaySound(sound, this.sound);
    }

    IEnumerator Reload(Weapon weapon, int currentAmmo, bool byOne)
    {
        yield return new WaitForSeconds(0.75f);

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
        else if(Input.GetKeyDown(KeyCode.E)) GadgetSpawn();
        else if(Input.GetKeyDown(KeyCode.Tab)) Pickup();
        else if(Input.GetKeyDown(KeyCode.F)) SwapHand();
    }

    private void SwapHand()
    {
        //if(gameObject.transform.localScale.x == -1f) gameObject.transform.localScale = new Vector3(1f, 1f, 0f);
        //else 
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
        if(gadgets.equippedGadget != null && gadgets.equippedGadget.hasGadget && gadgets.equippedGadget.isSpawned)
        {
            /*if(controlState == ControlState.Player) controlState = ControlState.Gadget;
            else controlState = ControlState.Player;*/
            controlState = controlState == ControlState.Player ? ControlState.Gadget : ControlState.Player;
        }
        else controlState = ControlState.Player;
    }

    private void GadgetSpawn()
    {
        if (gadgets.equippedGadget != null && gadgets.equippedGadget.hasGadget)
        {
            if(!gadgets.equippedGadget.isSpawned)
            {
                gadget = Instantiate(gadgets.equippedGadget.prefab, gameObject.transform.position, Quaternion.identity);
                gadgets.equippedGadget.isSpawned = true;
                StartGadgetTimer(10f);
            }
            else if(Vector2.Distance(gameObject.transform.position, gadget.transform.position) < 2f)
            {
                gadget.GetComponent<RC>().DestroyRC();
                gadgets.equippedGadget.isSpawned = false;
                StopGadgetTimer();
            }
        }
    }

    private void SelectWeapon(Weapon weapon) /* Funkcia, ktorá zmení premennú aktuálne držanej veci */
    {
        if(weapon == null || wl.selected == weapon) return;
        body.SetInteger("item", weapon.id); /* Nastaví premennú v Animátorovi, ktorý začne prehrávať príslušnú animáciu, ktorá bola premennej pridelená */
        //holdingItem = weapon.id;
        wl.selected = weapon;
        sounds.PlaySound(weapon.drawSound, sound);
    }

    private void CheckPlayerStatus()
    {
        //if(GetSelectedItem() == 1 && !wl.GetWeaponByID(1).HasWeapon()) SelectItem(0);
        //else if (GetSelectedItem() == 2 && !wl.GetWeaponByID(2).HasWeapon()) SelectItem(0);
        //else if (GetSelectedItem() == 3 && !wl.GetWeaponByID(3).HasWeapon()) SelectItem(0);
        //if(wl.selected != wl.primary && wl.selected != wl.secondary) SelectWeapon(wl.weapons[0]);
    }

    private void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        float horizontalSpeed;
        float verticalSpeed;

        if(horizontal != 0f || vertical != 0f)
        {
            horizontalSpeed = horizontal * speed;
            verticalSpeed = vertical * speed;

            if(Input.GetKey(KeyCode.LeftShift)) //Chôdza
            {
                horizontalSpeed = horizontalSpeed / 2f;
                verticalSpeed = verticalSpeed / 2f;
                speedAnimation = (Mathf.Abs(horizontal) + Mathf.Abs(vertical)) / 2f;
                feet.speed = 0.4f;
            }
            else //Šprint
            {
                speedAnimation = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                feet.speed = 1f;

                foreach(Enemy enemy in Enemy.GetEnemyList())
                {
                    if(enemy != null && Vector2.Distance(transform.position, enemy.transform.position) < (4.5f + GlobalValues.difficulty))
                    {
                        enemy.TurnAtObject(gameObject, false);
                    }
                }
            }

            feet.SetFloat("Speed", speedAnimation);
        }
        else
        {
            horizontalSpeed = 0f;
            verticalSpeed = 0f;
            feet.SetFloat("Speed", 0);
        }

        rb.velocity = new Vector2(horizontalSpeed, verticalSpeed);
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

    public void GiveMoney(int money)
    {
        this.money += money;
    }

    public void SetMoney(int money)
    {
        this.money = money;
    }

    public void TakeMoney(int money)
    {
        this.money = money;
    }

    public void OnDeath()
    {
        menu.ShowDeathMenu();
    }

    private void StartGadgetTimer(float time)
    {
        timerIns.GetComponent<Slider>().maxValue = time;
        timerIns.GetComponent<Slider>().value = time;
        gadgetTimer = new GadgetTimer(time, true);
        timerIns.SetActive(true);
        timerIns.transform.Find("BarText").gameObject.GetComponent<Text>().text = time.ToString();
    }

    private void StopGadgetTimer()
    {
        gadgetTimer.active = false;
        timerIns.SetActive(false);
        GadgetTimerStopped();
    }

    private void UpdateGadgetTimer()
    {
        if(gadgetTimer.active)
        {
            gadgetTimer.timeLeft -= Time.deltaTime;
            timerIns.transform.Find("BarText").gameObject.GetComponent<Text>().text = ((int) gadgetTimer.timeLeft).ToString();
            timerIns.GetComponent<Slider>().value = gadgetTimer.timeLeft;
            if(gadgetTimer.timeLeft <= 0) StopGadgetTimer();
        }
    }

    private void GadgetTimerStopped()
    {
        gadget.GetComponent<RC>().DestroyRC();
        gadgets.equippedGadget.isSpawned = false;
        GadgetControl();
    }

    ///////////////////////////
    // Ukladanie a načítanie //
    ///////////////////////////

    public void LoadSave()
    {
        healthSystem.SetHealth(PlayerPrefs.GetInt("health", 100));
        healthSystem.SetArmor(PlayerPrefs.GetInt("armor", 100));
        healthSystem.SetMaxHealth(100 + PlayerPrefs.GetInt("healthLvl", 0) * 5);
        healthSystem.SetMaxShield(100 + PlayerPrefs.GetInt("shieldLvl", 0) * 5);
        GlobalValues.fov = (byte) PlayerPrefs.GetInt("fovLvl", 0);
        GlobalValues.difficulty = (byte) PlayerPrefs.GetInt("difficulty", 0);
        money = PlayerPrefs.GetInt("money", 0);

        if(PlayerPrefs.GetInt("primary", 0) != 0) wl.primary = wl.GetWeaponByID(PlayerPrefs.GetInt("primary", 0));
        if(PlayerPrefs.GetInt("secondary", 0) != 0) wl.secondary = wl.GetWeaponByID(PlayerPrefs.GetInt("secondary", 0));
        Weapon lastSelect = wl.GetWeaponByID(PlayerPrefs.GetInt("selected", 0));

        for(int i = 1; i < GlobalValues.WEAPONS_COUNT; ++i)
        {
            LoadWeapon(wl.GetWeaponByID(i));
        }

        currentLevel = PlayerPrefs.GetInt("level", 1);
        SelectWeapon(lastSelect);
    }

    private void LoadWeapon(Weapon weapon)
    {
        weapon.ammo = PlayerPrefs.GetInt("ammo" + weapon.id, 0);
        weapon.magazine = PlayerPrefs.GetInt("magazine" + weapon.id, 0);
        //weapon.SetWeapon(MathFunctions.IntToBool(PlayerPrefs.GetInt("has" + weapon.id, 0)));
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("health", healthSystem.GetHealth());
        PlayerPrefs.SetInt("armor", healthSystem.GetArmor());
        PlayerPrefs.SetInt("money", money);

        if(wl.primary != null) PlayerPrefs.SetInt("primary", wl.primary.id);
        else PlayerPrefs.SetInt("primary", 0);
        if(wl.secondary != null) PlayerPrefs.SetInt("secondary", wl.secondary.id);
        else PlayerPrefs.SetInt("secondary", 0);
        if(wl.selected != null) PlayerPrefs.SetInt("selected", wl.selected.id);
        else PlayerPrefs.SetInt("selected", 0);

        for(int i = 1; i < GlobalValues.WEAPONS_COUNT; ++i)
        {
            SaveWeapon(wl.GetWeaponByID(i));
        }
    }

    private void SaveWeapon(Weapon weapon)
    {
        PlayerPrefs.SetInt("ammo" + weapon.id, weapon.ammo);
        PlayerPrefs.SetInt("magazine" + weapon.id, weapon.magazine);
        //PlayerPrefs.SetInt("has" + weapon.id, MathFunctions.BoolToInt(weapon.HasWeapon()));
    }
}
