using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheEnd : MonoBehaviour
{
    void Awake()
    {
        PlayerPrefs.SetInt("level", 0);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
