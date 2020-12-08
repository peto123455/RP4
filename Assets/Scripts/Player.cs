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
                WeaponList.Weapon weapon = GetWeaponByID(holdingItem);
                if(!weapon.isCooldown && !weapon.playerIgnoreCooldown || weapon.playerIgnoreCooldown && Input.GetButtonDown("Fire1"))
                {
                    if(weapon == wl.knife)
                    {
                        weapon.SetCooldown(weapon.cooldownTime);
                        body.Play(weapon.shootAnimationName);
                        RaycastHit2D checkRay = Physics2D.Raycast(gameObject.transform.position, mousePos - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), 1.5f, enemyLayer);
                        if (checkRay.collider != null && checkRay.collider.tag == "Enemy")
                        {
                            Enemy enemy = checkRay.collider.GetComponent<Enemy>();
                            enemy.TakeDamage(100, false);
                            sounds.PlaySound(weapon.sound, sound);
                            GameObject hitInstance = Instantiate(hitEffect, checkRay.point, Quaternion.identity);
                            Destroy(hitInstance, 2f);
                        }
                    }
                    else if(weapon.magazine > 0)
                    {
                        weapon.SetCooldown(weapon.cooldownTime);
                        Shoot(weapon);
                        weapon.magazine -= 1;
                        sounds.PlaySound(weapon.sound, sound);
                        body.Play(weapon.shootAnimationName);
                    }
                    else if (weapon.ammo > 0) ReloadGun(weapon);
                }
            }
        }
    }

    private void UpdateCooldowns()
    {
        for(byte i = 0; i < 4; ++i)
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
        else if (Input.GetKey(KeyCode.Alpha4) && wl.shotgun.hasWeapon) SelectItem(3); //AK-47

        if (Input.GetKeyDown(KeyCode.R) && holdingItem != 0) ReloadGun(GetWeaponByID(holdingItem)); //Reload
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
        else if (GetSelectedItem() == 3 && !HasWeapon(3)) SelectItem(0);
    }

    void Shoot(WeaponList.Weapon weapon) /* Funkcia, ktorá sa vykoná ak zbraň je nabitá a pripravená k streľbe */
    {
        GameObject bullet = Instantiate(bulletPistol, firePoint.position, firePoint.rotation);

        switch (weapon.bulletType)
        {
            case 1:
                if (Random.Range(0, 10) == 0) bullet.GetComponent<Bullet>().SetBulletDamage(40, true);
                else bullet.GetComponent<Bullet>().SetBulletDamage(20, false);
                break;
            case 2:
                if (Random.Range(0, 5) == 0) bullet.GetComponent<Bullet>().SetBulletDamage(50, true);
                else bullet.GetComponent<Bullet>().SetBulletDamage(25, false);
                break;
            case 3: //shotgun slug
                if (Random.Range(0, 2) == 0) bullet.GetComponent<Bullet>().SetBulletDamage(60, true);
                else bullet.GetComponent<Bullet>().SetBulletDamage(40, false);
                break;
        }

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.AddForce(firePoint.up * 25f, ForceMode2D.Impulse);
    }

    private void ReloadGun(WeaponList.Weapon weapon) /* Funkcia, ktorá prebije zbraň */
    {

        if (weapon == wl.knife) return;
        else if (weapon.magazine != weapon.maxMagazine && !weapon.isCooldown)
        {
            if(weapon == wl.shotgun)
            {
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

    IEnumerator DelayedSound(float delay, int sound)
    {
        yield return new WaitForSeconds(delay);

        sounds.PlaySound(sound, this.sound);
    }

    IEnumerator ShotgunReload()
    {
        yield return new WaitForSeconds(0.75f);

        if(holdingItem == 3) ReloadGun(GetWeaponByID(holdingItem));
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
            case 3:
                return wl.shotgun;
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

        LoadWeapon(wl.glock);
        LoadWeapon(wl.ak);
        LoadWeapon(wl.shotgun);

        currentLevel = PlayerPrefs.GetInt("level", 1);
    }

    private void LoadWeapon(WeaponList.Weapon weapon)
    {
        weapon.ammo = PlayerPrefs.GetInt("ammo" + weapon.name, 0);
        weapon.magazine = PlayerPrefs.GetInt("magazine" + weapon.name, 0);
        weapon.SetWeapon(IntToBool(PlayerPrefs.GetInt("has" + weapon.name, 0)));
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("health", health);
        PlayerPrefs.SetInt("armor", armor);

        SaveWeapon(wl.glock);
        SaveWeapon(wl.ak);
        SaveWeapon(wl.shotgun);
    }

    private void SaveWeapon(WeaponList.Weapon weapon)
    {
        PlayerPrefs.SetInt("ammo" + weapon.name, weapon.ammo);
        PlayerPrefs.SetInt("magazine" + weapon.name, weapon.magazine);
        PlayerPrefs.SetInt("has" + weapon.name, BoolToInt(weapon.HasWeapon()));
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
