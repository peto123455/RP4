using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Sprite imageKnife, imageGlock, imageAK, imageShotgun;
    [SerializeField] private Image image;
    [SerializeField] private Text ammoText, healthText, shieldText, levelText, popupText;
    [SerializeField] private Slider healthbar, shieldbar;
    [SerializeField] private GameObject popupWindow;

    private float popupTime;

    private Player player;

    void Awake()
    {
        popupTime = 0;
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        levelText.text = "Level " + player.currentLevel.ToString();
        //Životy
        healthbar.value = player.health;
        healthText.text = player.health.ToString();
        //Štít
        shieldbar.value = player.armor;
        shieldText.text = player.armor.ToString();
        //Zbrane
        switch (player.GetSelectedItem())
        {
            case 0:
                ammoText.text = "KNIFE";
                image.sprite = imageKnife;
                break;
            case 1:
                ShowWeapon(imageGlock, player.wl.glock.magazine, player.wl.glock.ammo);
                break;
            case 2:
                ShowWeapon(imageAK, player.wl.ak.magazine, player.wl.ak.ammo);
                break;
            case 3:
                ShowWeapon(imageShotgun, player.wl.shotgun.magazine, player.wl.shotgun.ammo);
                break;
        }


        if(popupWindow.activeSelf && popupTime < Time.time) popupWindow.SetActive(false);
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
