using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingScript : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 1f); //Po vytvorení objektu sa zničí o 0.4 sekundy
    }

    public void SetText(string text, float offset, int color)
    {
        Vector3 position = transform.position; //Zoberie svoju aktuálnu pozíciu
        position = new Vector3(position.x, position.y + offset, position.z); //Pripočíta k nej odchylku
        transform.position = position; //Aktualizuje pozíciu
        TextMeshPro tmpText = gameObject.transform.GetChild(0).GetComponent<TextMeshPro>(); //Zoberie komponent TextMeshPro zo svojho childa s indexom 0
        tmpText.text = text; //Nastaví text

        switch(color) //Switch s farbami
        {
            case 0:
                break;
            case 1:
                tmpText.color = new Color(255, 0, 0, 255);
                break;
            case 2:
                tmpText.color = new Color(255, 100, 0, 255);
                break;
        }
    }
}
