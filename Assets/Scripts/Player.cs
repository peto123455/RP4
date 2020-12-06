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
    public int health, armor, currentLevel; /* Deklaruje zdravie a štít a nastaví na 100 */
    public LayerMask enemyLayer;
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
    public WeaponList wl;
    private Gadgets gadgets;
    private GameObject gadget;
    private ControlState controlState = ControlState.Player;

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
        wl = GetComponent<WeaponList>();
        sounds = GetComponent<Sounds>();
        sound = GetComponent<AudioSource>();
        gadgets = GetComponent<Gadgets>();

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
        /* Kontrola kláves */
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
            /* KNIFE */
            if (holdingItem == 0 && !wl.knife.isCooldown) /* Podmienka, ktoá kontroluje či hráč drží RMB, má v ruke ňôž a či je pripravený */
            {
                wl.knife.isCooldown = true;
                wl.knife.cooldown = wl.knife.cooldownTime;
                body.Play("Player_knife_attack");
                RaycastHit2D checkRay = Physics2D.Raycast(gameObject.transform.position, mousePos - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), 1.5f, enemyLayer);
                if (checkRay.collider != null && checkRay.collider.tag == "Enemy")
                {
                    Enemy enemy = checkRay.collider.GetComponent<Enemy>();
                    enemy.TakeDamage(100, false);
                    sounds.PlaySound(1, sound);
                    GameObject hitInstance = Instantiate(hitEffect, checkRay.point, Quaternion.identity);
                    Destroy(hitInstance, 2f);
                }
            }
            /* GLOCK 21 */
            else if (holdingItem == 1 && !wl.glock.isCooldown && Input.GetButtonDown("Fire1"))
            {
                if (wl.glock.magazine > 0)
                {
                    wl.glock.magazine -= 1;
                    sounds.PlaySound(0, sound);
                    //wl.glock.isCooldown = true;
                    //wl.glock.cooldown = wl.glock.cooldownTime;
                    Shoot(1);
                    //StartCoroutine(Shoot(firePoint.position, mousePos - new Vector2(firePoint.position.x, firePoint.position.y)));
                    body.Play("Player_handgun_shoot");
                    //if (magazineGlock <= 0) ReloadGun(1);
                }
                else if (wl.glock.ammo > 0) ReloadGun(1);
            }
            /* AK-47 */
            else if (holdingItem == 2 && !wl.ak.isCooldown)
            {
                if (wl.ak.magazine > 0)
                {
                    wl.ak.magazine -= 1;
                    sounds.PlaySound(3, sound);
                    wl.ak.isCooldown = true;
                    wl.ak.cooldown = wl.ak.cooldownTime;
                    Shoot(2);
                    body.Play("Player_rifle_shoot");
                }
                else if (wl.ak.ammo > 0) ReloadGun(2);
            }
        }
    }

    private void UpdateCooldowns()
    {
        /*
        if (knife.isCooldown)
        {
            cooldownKnife -= Time.deltaTime;

            if (cooldownKnife <= 0)
            {
                cooldownKnife = 0;
                knife.isCooldown = false;
            }
        }

        if (glock.isCooldown)
        {
            cooldownHandgun -= Time.deltaTime;

            if (cooldownHandgun <= 0)
            {
                cooldownHandgun = 0;
                glock.isCooldown = false;
            }
        }

        if (ak.isCooldown)
        {
            cooldownRifle -= Time.deltaTime;

            if (cooldownRifle <= 0)
            {
                cooldownRifle = 0;
                ak.isCooldown = false;
            }
        }
        */

        for(byte i = 0; i < 3; ++i)
        {
            UpdateCooldown(GetWeaponByID(i));
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

    private void CheckKeys() /* Funkcia, ktorá sa volá 50 krát za sekundu nezávisle od snímkov obrazovky a kontroluje stlačené klávesi */
    {
        if (Input.GetKey(KeyCode.Alpha1)) SelectItem(0); //Knife
        else if (Input.GetKey(KeyCode.Alpha2) && wl.glock.hasWeapon) SelectItem(1); //Glock
        else if (Input.GetKey(KeyCode.Alpha3) && wl.ak.hasWeapon) SelectItem(2); //AK-47

        if (Input.GetKeyDown(KeyCode.R) && holdingItem != 0) ReloadGun(holdingItem); //Reload
        else if(Input.GetKeyDown(KeyCode.E)) GadgetSpawn();
    }

    private void UniversalKeys()
    {
        if(Input.GetKeyDown(KeyCode.Q)) GadgetControl(); 
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

    private void SelectItem(int item) /* Funkcia, ktorá zmení premennú aktuálne držanej veci */
    {
        // Zbrane: 0:Knife 1:Glock-21
        body.SetInteger("item", item); /* Nastaví premennú v Animátorovi, ktorý začne prehrávať príslušnú animáciu, ktorá bola premennej pridelená */
        holdingItem = item;
    }

    private void CheckPlayerStatus()
    {
        if(GetSelectedItem() == 1 && !HasWeapon(1)) SelectItem(0); /* Ak nemá glock, dá mu ho preč z ruky */
        else if (GetSelectedItem() == 2 && !HasWeapon(2)) SelectItem(0);
    }

    /*IEnumerator Shoot(Vector2 shootPos, Vector2 shootDir)  //Toto si tu ponechávam, keby som to niekedy plánovať využiť
    {
        RaycastHit2D bullet = Physics2D.Raycast(shootPos, shootDir);
        Debug.DrawRay(shootPos, shootDir, Color.green);
        if (bullet.collider != null)
        {
            Enemy target = bullet.collider.GetComponent<Enemy>();
            if (target != null)
            {
                target.TakeDamage(10);
            }
        }
        lr.SetPosition(0, firePoint.position);
        lr.SetPosition(1, mousePos);

        lr.enabled = true;

        yield return new WaitForSeconds(0.02f);

        lr.enabled = false;
    }*/

    void Shoot(int type) /* Funkcia, ktorá sa vykoná ak zbraň je nabitá a pripravená k streľbe */
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
        }

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.AddForce(firePoint.up * 25f, ForceMode2D.Impulse);
    }

    private void ReloadGun(int gun) /* Funkcia, ktorá prebije zbraň */
    {
        switch (gun) /* Switch, ktorý zisťuje, ktorú zbraň chce hráč prebiť */
        {
            case 1:
                if (wl.glock.magazine != wl.glock.maxMagazine && !wl.glock.isCooldown)
                {
                    wl.glock.isCooldown = true;
                    wl.glock.cooldown = wl.glock.cooldownReload;
                    CalcAmmo(ref wl.glock.magazine, ref wl.glock.ammo, wl.glock.maxMagazine);
                    if (wl.glock.magazine != 0)
                    {
                        sounds.PlaySound(2, sound);
                        body.Play("Player_handgun_reload");
                    }
                }
                break;
            case 2:
                if (wl.ak.magazine != wl.ak.maxMagazine && !wl.ak.isCooldown)
                {
                    wl.ak.isCooldown = true;
                    wl.ak.cooldown = wl.ak.cooldownReload;
                    CalcAmmo(ref wl.ak.magazine, ref wl.ak.ammo, wl.ak.maxMagazine);
                    if (wl.ak.magazine != 0)
                    {
                        sounds.PlaySound(2, sound);
                        body.Play("Player_rifle_reload");
                    }
                }
                break;
        }
    }

    private void CalcAmmo(ref int magazine, ref int ammo, int maximum) /* Funkcia, ktorá prehodí náboje do zásobníka */
    {
        ammo += magazine;
        if(ammo >= maximum)
        {
            magazine = maximum;
            ammo -= maximum;
        }
        else
        {
            magazine = ammo;
            ammo = 0;
        }
    }

    public void TakeDamage(int damage, bool critical) /* Funkcia, ktorá odobere hráčovi životy */
    {
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
    }

    /*public class Ammo
    {
        public int magazine;
        public int ammo;
        public float cooldown;
        public bool isCooldown = false;
        public bool hasWeapon = false;

        public Ammo(int magazine, int ammo)
        {
            this.magazine = magazine;
            this.ammo = ammo;
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

    public void GiveAmmo(int type, int amount) /* Funkcia, ktorá po zavolaní dá hráčovi náboje do príslušnej zbrane */
    {
        GetWeaponByID(type).ammo += amount;
    }

    public void SetAmmo(int type, int amount)
    {
        GetWeaponByID(type).ammo = amount;
    }

    public void SetWeapon(int type, bool has)
    {
        GetWeaponByID(type).SetWeapon(has);
    }

    public bool HasWeapon(int type)
    {
        return GetWeaponByID(type).HasWeapon();
    }

    private WeaponList.Weapon GetWeaponByID(int type)
    {
        switch(type)
        {
            case 0:
                return wl.knife;
            case 1:
                return wl.glock;
            case 2:
                return wl.ak;
        }
        return null;
    }

    public int GetSelectedItem()
    {
        return holdingItem;
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public int GetArmor()
    {
        return armor;
    }

    public void SetArmor(int armor)
    {
        this.armor = armor;
    }

    private void OnDeath()
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

    public void LoadSave()
    {
        health = PlayerPrefs.GetInt("health", 100);
        armor = PlayerPrefs.GetInt("armor", 0);

        wl.glock.ammo = PlayerPrefs.GetInt("ammoGlock", 0);
        wl.ak.ammo = PlayerPrefs.GetInt("ammoAK", 0);

        wl.glock.magazine = PlayerPrefs.GetInt("magazineGlock", 0);
        wl.ak.magazine = PlayerPrefs.GetInt("magazineAK", 0);

        wl.glock.SetWeapon(IntToBool(PlayerPrefs.GetInt("hasGlock", 0)));
        wl.ak.SetWeapon(IntToBool(PlayerPrefs.GetInt("hasAK", 0)));

        currentLevel = PlayerPrefs.GetInt("level", 1);
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("health", health);
        PlayerPrefs.SetInt("armor", armor);

        PlayerPrefs.SetInt("ammoGlock", wl.glock.ammo);
        PlayerPrefs.SetInt("ammoAK", wl.ak.ammo);

        PlayerPrefs.SetInt("magazineGlock", wl.glock.magazine);
        PlayerPrefs.SetInt("magazineAK", wl.ak.magazine);

        PlayerPrefs.SetInt("hasGlock", BoolToInt(wl.glock.HasWeapon()));
        PlayerPrefs.SetInt("hasAK", BoolToInt(wl.ak.HasWeapon()));
    }

    private bool IntToBool(int integer)
    {
        return integer == 1;
    }

    private int BoolToInt(bool boolean)
    {
        return boolean ? 1:0;
    }
}
