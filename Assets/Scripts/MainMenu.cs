using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject continueButton;
    private bool isKeyChecked = false;

    private WeaponList wl = new WeaponList();

    void Awake()
    {
        if(PlayerPrefs.GetInt("level", 0) == 0) //Zistuje, či už má vytvorenú hru, ak nie, nezobrazí tlačidlo "Continue"
        {
            continueButton.SetActive(false);
        }
    }

    public void StartNewGame() //Funkcia, ktorá sa zavolá po stlačení tlačidla "New Game"
    {
        if(isKeyChecked)
        {
            CreateGame();
            SceneManager.LoadScene(1); //Načítanie scény s indexom 1
        }
    }

    public void ExitGame()
    {
        Application.Quit(); //Ukončenie Hry
    }

    private void CreateGame() // Nastaví zákl. hodnoty pre novú hru.
    {
        /* Základné */
        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.SetInt("health", 100);
        PlayerPrefs.SetInt("armor", 0);
        PlayerPrefs.SetInt("money", 0);
        PlayerPrefs.SetInt("primary", 0);
        PlayerPrefs.SetInt("secondary", 0);
        PlayerPrefs.SetInt("selected", 0);
        for(int i = 1; i < GlobalValues.WEAPONS_COUNT; ++i)
        {
            CreateWeaponSave(wl.GetWeaponByID(i));
        }
    }

    private void CreateWeaponSave(WeaponList.Weapon weapon)
    {
        PlayerPrefs.SetInt("ammo" + weapon.id, 0);
        PlayerPrefs.SetInt("magazine" + weapon.id, 0);
        PlayerPrefs.SetInt("has" + weapon.id, 0);
    }

    public void Continue()
    {
        if(isKeyChecked) SceneManager.LoadScene(PlayerPrefs.GetInt("level", 0));
    }

    public void KeyChecked()
    {
        this.isKeyChecked = true;
    }
}
