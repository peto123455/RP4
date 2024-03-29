﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused = false;
    [SerializeField] private GameObject pauseMenu, deathMenu, completeMenu, settingsMenu, UI;

    void Awake()
    {
        Time.timeScale = 1f;
        UI = GameObject.Find("UI");
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
        if(!deathMenu.activeSelf && !completeMenu.activeSelf)
        {
            pauseMenu.SetActive(true);
            deathMenu.SetActive(false);
            completeMenu.SetActive(false);
            settingsMenu.SetActive(false);

            UI.SetActive(false);
        }
    }

    public void ShowDeathMenu()
    {
        Time.timeScale = 0f;
        isPaused = true;

        deathMenu.SetActive(true);
        pauseMenu.SetActive(false);
        completeMenu.SetActive(false);
        settingsMenu.SetActive(false);

        UI.SetActive(false);
    }

    public void ShowCompleteMenu()
    {
        Time.timeScale = 0f;
        isPaused = true;

        deathMenu.SetActive(false);
        pauseMenu.SetActive(false);
        completeMenu.SetActive(true);
        settingsMenu.SetActive(false);

        UI.SetActive(false);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("level", 1));
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;

        pauseMenu.SetActive(false);
        deathMenu.SetActive(false);
        completeMenu.SetActive(false);
        settingsMenu.SetActive(false);

        UI.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        isPaused = false;

        Enemy.enemyList.Clear();

        SceneManager.LoadScene(PlayerPrefs.GetInt("level"));
    }

    public void Quit()
    {
        Application.Quit();
    }
}

