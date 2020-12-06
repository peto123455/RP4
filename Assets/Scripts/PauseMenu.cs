using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused = false;
    [SerializeField] private GameObject pauseMenu, deathMenu;

    void Awake()
    {
        Time.timeScale = 1f;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else ShowPauseMenu();
        }
    }

    public void ShowPauseMenu()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if(!deathMenu.activeSelf)
        {
            pauseMenu.SetActive(true);
            deathMenu.SetActive(false);
        }
    }

    public void ShowDeathMenu()
    {
        Time.timeScale = 0f;
        isPaused = true;
        deathMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenu.SetActive(false);
        deathMenu.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}

