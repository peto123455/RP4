using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Sprite imageKnife, imageGlock, imageAK, imageShotgun;
    [SerializeField] private Image image;
    [SerializeField] private Text ammoText, healthText, shieldText, levelText, popupText, moneyText;
    [SerializeField] private Slider healthbar, shieldbar;
    [SerializeField] private GameObject popupWindow;

    private List<Sprite> sprites = new List<Sprite>();

    private float popupTime;

    int healthLvl;

    private Player player;

    void Awake()
    {
        popupTime = 0;
        player = GameObject.Find("Player").GetComponent<Player>();

        sprites.Add(imageKnife);
        sprites.Add(imageGlock);
        sprites.Add(imageAK);
        sprites.Add(imageShotgun);
    }

    void Update()
    {
        UpdateUI();

        if(popupWindow.activeSelf && popupTime < Time.time) popupWindow.SetActive(false);
    }

    private void UpdateUI()
    {
        levelText.text = "Level " + player.currentLevel.ToString();
        //Životy
        healthbar.maxValue = player.healthSystem.GetMaxHealth();
        healthbar.value = player.healthSystem.GetHealth();
        healthText.text = player.healthSystem.GetHealth().ToString();
        //Štít
        shieldbar.value = player.healthSystem.GetArmor();
        shieldText.text = player.healthSystem.GetArmor().ToString();
        //Peniaze
        moneyText.text = player.money.ToString();
        //Zbrane
        Weapon weapon = player.wl.GetHoldingWeapon();
        if(weapon.id == 0)
        {
            image.sprite = imageKnife;
            ammoText.text = "KNIFE";
            return;
        }
        ShowWeapon(sprites[weapon.id], weapon.magazine, weapon.ammo);
    }

    public void ShowText(float time, string text)
    {
        popupWindow.SetActive(true);
        popupText.text = text;
        popupTime = Time.time + time;
    }

    public void ShowWeapon(Sprite sprite, int magazine, int ammo)
    {
        image.sprite = sprite;
        ammoText.text = magazine.ToString() + " / " + ammo.ToString();
    }
}
