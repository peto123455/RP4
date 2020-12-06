using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMessageTrigger : MonoBehaviour
{
    [SerializeField] private string text;
    [SerializeField] private float time = 7f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameObject.Find("UI").GetComponent<WeaponUI>().ShowText(time, text.Replace("/n", System.Environment.NewLine)); // \n (new line) nefunguje, vytváram zaň náhradu /n
            Destroy(gameObject);
        }
    }
}
