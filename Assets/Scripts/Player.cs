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
    [SerializeField] private Transform firePoint;
    [SerializeField] private PauseMenu menu;
    [SerializeField] private GameObject timerIns;

    public Animator feet, body;
    public float speed = 5.0f;
    public GameObject bulletPistol, hitEffect, floatingText;
    public int currentLevel, money;
    public LayerMask enemyLayer, collisionLayer;
    private AudioSource sound = new AudioSource();

    //public Ammo knife = new Ammo(0, 0); /* Slúži len na zistovanie cooldownu na nožíku */
    //public Ammo glock = new Ammo(13, 39); /* Hráčove náboje a stav cooldownu v zbrani Glock */
    //public Ammo ak = new Ammo(30, 120);

    private float speedAnimation;
    //private float[] cooldown = { 0f, 0f, 0f }; //0 Knife, 1 Glock, 2 Rifle

    private int holdingItem = 0;
    private Vector2 mouseVec, mousePos;
    private Sounds sounds;
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

    void Start() /* Funkcia, ktorá sa volá pri spustení skriptu */
    {
        rb = GetComponent<Rigidbody2D>(); /* Zoberie komponent Rigidbody2D a uloží ho do rb*/
        //wl = GetComponent<WeaponList>();
        sounds = GetComponent<Sounds>();
        sound = GetComponent<AudioSource>();
        gadgets = GetComponent<Gadgets>();
        healthSystem = GetComponent<HealthSystem>();

        gadgets.SetGadget(gadgets.rcCar, true);
        gadgets.EquipGadget(gadgets.rcCar);

        LoadSave();
    }

    void Update() /* Funkcia, ktorá sa vykonáva každý snímok */
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
        }
        else if (controlState == ControlState.Gadget)
        {
            Camera.main.transform.position = new Vector3(gadget.transform.position.x, gadget.transform.position.y, -10);
            CheckKeysGadget();
        }

        UniversalKeys();
        UpdateGadgetTimer();
    }

    void LateUpdate() /* LateUpdate sa vykonáva až po dokončení všetkých Updatov */
    {
        UpdateMousePos();
        /* Stará sa o nastavenie pozície a rotácie FOV */
        fov.SetPosition(transform.position);
        if(controlState == ControlState.Player) fov.SetDirection((Mathf.Atan2(mouseVec.x, mouseVec.y) * Mathf.Rad2Deg) - 90f);
    }

    private void FixedUpdate() /* Funkcia, ktorá sa pravidelne vykonáva nezávisle od počtu snímkov za sekundu */
    {
        if(controlState == ControlState.Player)
        {
            Movement(); //Pohyb hráča
            if(gadget != null) gadget.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        }
        else if(controlState == ControlState.Gadget)
        {
            GadgetMovement();
            rb.velocity = new Vector2(0f, 0f);
            feet.SetFloat("Speed", 0);
        }
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
            if(holdingItem < 100) //Všetky zbrane
            {
                WeaponList.Weapon weapon = wl.GetWeaponByID(holdingItem);
                if(!weapon.isCooldown && !weapon.playerIgnoreCooldown || weapon.playerIgnoreCooldown && Input.GetButtonDown("Fire1"))
                {
                    if(weapon == wl.GetWeaponByID(0))
                    {
                        weapon.SetCooldown(weapon.cooldownTime);
                        body.Play(weapon.shootAnimationName);
                        RaycastHit2D checkRay = Physics2D.Raycast(gameObject.transform.position, mousePos - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), 1.5f, enemyLayer);
                        if (checkRay.collider != null && checkRay.collider.tag == "Enemy")
                        {
                            Enemy enemy = checkRay.collider.GetComponent<Enemy>();
                            enemy.healthSystem.TakeDamage(100, false, gameObject);
                            sounds.PlaySound(weapon.sound, sound);
                            GameObject hitInstance = Instantiate(hitEffect, checkRay.point, Quaternion.identity);
                            Destroy(hitInstance, 2f);
                        }
                    }
                    else if(weapon.magazine > 0)
                    {
                        Vector2 rayDirection = MathFunctions.InvertVector(mousePos - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));

                        RaycastHit2D checkRay = Physics2D.Raycast(firePoint.transform.position, rayDirection, 1f, collisionLayer); //Slúži na detekciu, či hráč sa nepokúša strielať cez stenu
                        if(checkRay.collider == null)
                        {
                            weapon.SetCooldown(weapon.cooldownTime);
                            Shoot(weapon);
                            weapon.magazine -= 1;
                            sounds.PlaySound(weapon.sound, sound);
                            body.Play(weapon.shootAnimationName);
                        }
                    }
                    else if (weapon.ammo > 0) ReloadGun(weapon);
                }
            }
        }
    }

    void Shoot(WeaponList.Weapon weapon) /* Funkcia, ktorá sa vykoná ak zbraň je nabitá a pripravená k streľbe */
    {
        GameObject bulletPrefab = Instantiate(bulletPistol, firePoint.position, firePoint.rotation);

        Bullet bullet = bulletPrefab.GetComponent<Bullet>();
        bullet.SetShooter(gameObject);

        switch (weapon.bulletType)
        {
            case 1:
                if (Random.Range(0, 10) == 0) bullet.GetComponent<Bullet>().SetBulletDamage(40, true);
                else bullet.SetBulletDamage(20, false);
                break;
            case 2:
                if (Random.Range(0, 5) == 0) bullet.GetComponent<Bullet>().SetBulletDamage(50, true);
                else bullet.SetBulletDamage(25, false);
                break;
            case 3:
                if (Random.Range(0, 2) == 0) bullet.GetComponent<Bullet>().SetBulletDamage(60, true);
                else bullet.SetBulletDamage(40, false);
                break;
        }

        Rigidbody2D rbBullet = bulletPrefab.GetComponent<Rigidbody2D>();
        rbBullet.AddForce(firePoint.up * 25f, ForceMode2D.Impulse);
    }

    private void ReloadGun(WeaponList.Weapon weapon) /* Funkcia, ktorá prebije zbraň */
    {

        if (weapon == wl.GetWeaponByID(0)) return;
        else if (weapon.magazine != weapon.maxMagazine && !weapon.isCooldown)
        {
            if(weapon == wl.GetWeaponByID(3))
            {
                if(weapon.ammo <= 0) return;
                weapon.SetCooldown(weapon.cooldownReload);
                weapon.ReloadByOne();
                sounds.PlaySound(5, sound);
                body.Play(weapon.reloadAnimationName);
                if(weapon.magazine == weapon.maxMagazine)
                {
                    weapon.SetCooldown(weapon.cooldownReload * 2);
                    StartCoroutine(DelayedSound(weapon.cooldownReload, 7));
                }
                else StartCoroutine(ShotgunReload());
            }
            else
            {
                weapon.SetCooldown(weapon.cooldownReload);
                weapon.Reload();
                if (weapon.magazine != 0)
                {
                    sounds.PlaySound(2, sound);
                    body.Play(weapon.reloadAnimationName);
                }
            }
        }
    }

    IEnumerator DelayedSound(float delay, int sound) //Slúži na prehratie zvuku s oneskorením
    {
        yield return new WaitForSeconds(delay);

        sounds.PlaySound(sound, this.sound);
    }

    IEnumerator ShotgunReload() //Slúži na opakované vloženie náboja do brokovnice, sa podmienky, že stále zbraň drží
    {
        yield return new WaitForSeconds(0.75f);

        if(holdingItem == 3) ReloadGun(wl.GetWeaponByID(holdingItem));
    }

    private void UpdateCooldowns()
    {
        for(byte i = 0; i < GlobalValues.WEAPONS_COUNT; ++i)
        {
            UpdateCooldown(wl.GetWeaponByID(i));
        }
    }

    private void UpdateCooldown(WeaponList.Weapon weapon)
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

    private void CheckKeys() //Funkcia na kontrolui kláves ak hráč ovláda postavu
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectItem(0); //Knife
        else if (Input.GetKeyDown(KeyCode.Alpha2) && wl.GetWeaponByID(1).hasWeapon) SelectItem(1); //Glock
        else if (Input.GetKeyDown(KeyCode.Alpha3) && wl.GetWeaponByID(2).hasWeapon) SelectItem(2); //AK-47
        else if (Input.GetKeyDown(KeyCode.Alpha4) && wl.GetWeaponByID(3).hasWeapon) SelectItem(3); //AK-47

        if (Input.GetKeyDown(KeyCode.R) && holdingItem != 0) ReloadGun(wl.GetWeaponByID(holdingItem)); //Reload
        else if(Input.GetKeyDown(KeyCode.E)) GadgetSpawn();
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
                StartGadgetTimer(30f);
            }
            else if(Vector2.Distance(gameObject.transform.position, gadget.transform.position) < 2f)
            {
                gadget.GetComponent<RC>().DestroyRC();
                gadgets.equippedGadget.isSpawned = false;
                StopGadgetTimer();
            }
        }
    }

    private void SelectItem(int item) /* Funkcia, ktorá zmení premennú aktuálne držanej veci */
    {
        if(item < 100 && holdingItem != item) sounds.PlaySound(wl.weapons[item].drawSound, sound);
        body.SetInteger("item", item); /* Nastaví premennú v Animátorovi, ktorý začne prehrávať príslušnú animáciu, ktorá bola premennej pridelená */
        holdingItem = item;
    }

    private void CheckPlayerStatus()
    {
        if(GetSelectedItem() == 1 && !wl.GetWeaponByID(1).HasWeapon()) SelectItem(0);
        else if (GetSelectedItem() == 2 && !wl.GetWeaponByID(2).HasWeapon()) SelectItem(0);
        else if (GetSelectedItem() == 3 && !wl.GetWeaponByID(3).HasWeapon()) SelectItem(0);
    }

    /*public void TakeDamage(int damage, bool critical, GameObject shotBy = null) /* Funkcia, ktorá odobere hráčovi životy */
    /*{
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
            OnDeath();
        }
    }*/

    private void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        speedAnimation = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
        feet.SetFloat("Speed", speedAnimation);
        rb.velocity = new Vector2(horizontal * speed, vertical * speed);
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

    //////////////////////////////
    // FUNKCIE PRE SPRÁVU HRÁČA //
    //////////////////////////////

    public WeaponList.Weapon GetHoldingWeapon()
    {
        return wl.GetWeaponByID(holdingItem);
    }

    public int GetSelectedItem()
    {
        return holdingItem;
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
    /////////////////////////////////////////////
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
        money = PlayerPrefs.GetInt("money", 0);

        for(int i = 1; i < GlobalValues.WEAPONS_COUNT; ++i)
        {
            LoadWeapon(wl.GetWeaponByID(i));
        }

        currentLevel = PlayerPrefs.GetInt("level", 1);
    }

    private void LoadWeapon(WeaponList.Weapon weapon)
    {
        weapon.ammo = PlayerPrefs.GetInt("ammo" + weapon.id, 0);
        weapon.magazine = PlayerPrefs.GetInt("magazine" + weapon.id, 0);
        weapon.SetWeapon(MathFunctions.IntToBool(PlayerPrefs.GetInt("has" + weapon.id, 0)));
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("health", healthSystem.GetHealth());
        PlayerPrefs.SetInt("armor", healthSystem.GetArmor());
        PlayerPrefs.SetInt("money", money);

        for(int i = 1; i < GlobalValues.WEAPONS_COUNT; ++i)
        {
            SaveWeapon(wl.GetWeaponByID(i));
        }
    }

    private void SaveWeapon(WeaponList.Weapon weapon)
    {
        PlayerPrefs.SetInt("ammo" + weapon.id, weapon.ammo);
        PlayerPrefs.SetInt("magazine" + weapon.id, weapon.magazine);
        PlayerPrefs.SetInt("has" + weapon.id, MathFunctions.BoolToInt(weapon.HasWeapon()));
    }
}
