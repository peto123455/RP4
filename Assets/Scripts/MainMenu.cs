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
        PlayerPrefs.SetInt("ammoGlock-21", 0);
        PlayerPrefs.SetInt("ammoAK-47", 0);
        PlayerPrefs.SetInt("ammoSpas-12", 0);

        PlayerPrefs.SetInt("magazineGlock-21", 0);
        PlayerPrefs.SetInt("magazineAK-47", 0);
        PlayerPrefs.SetInt("magazineSpas-12", 0);
        /* Zbrane */
        PlayerPrefs.SetInt("hasGlock-21", 0); //Nepodporuje bool, takže používam integer
        PlayerPrefs.SetInt("hasAK-47", 0);
        PlayerPrefs.SetInt("hasSpas-12", 0);
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
