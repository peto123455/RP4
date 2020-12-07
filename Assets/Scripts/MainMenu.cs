using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject continueButton;
    private bool isKeyChecked = false;

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
        /* Náboje */
        PlayerPrefs.SetInt("ammoGlock", 0);
        PlayerPrefs.SetInt("ammoAK", 0);
        PlayerPrefs.SetInt("ammoShotgun", 0);

        PlayerPrefs.SetInt("magazineGlock", 0);
        PlayerPrefs.SetInt("magazineAK", 0);
        PlayerPrefs.SetInt("magazineShotgun", 0);
        /* Zbrane */
        PlayerPrefs.SetInt("hasGlock", 0); //Nepodporuje bool, takže používam integer
        PlayerPrefs.SetInt("hasAK", 0);
        PlayerPrefs.SetInt("hasShotgun", 0);
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
