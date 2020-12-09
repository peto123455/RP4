using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.GetComponent<Player>().SaveGame();
            int nextLevel = PlayerPrefs.GetInt("level", 1) + 1;
            PlayerPrefs.SetInt("level", nextLevel);
            SceneManager.LoadScene(nextLevel);
        }
    }
}
