using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    [SerializeField] private  GameObject floatingText;
    [SerializeField] private Sprite glockSprite, akSprite, fakSprite, vestSprite;
    [SerializeField] private int type, amount;


    public void SetItem(int type, int amount) //Funkcia, pomocou ktorej zapíšeme hodnoty itemu (Čo to je za objekt a koľko ho je)
    {
        this.type = type;
        this.amount = amount;
        RefreshItem();
    }

    private void RefreshItem()
    {
        if (type == 1) GetComponent<SpriteRenderer>().sprite = glockSprite;
        else if (type == 2) GetComponent<SpriteRenderer>().sprite = akSprite;
        else if (type == 101) GetComponent<SpriteRenderer>().sprite = fakSprite;
        else if (type == 102) GetComponent<SpriteRenderer>().sprite = vestSprite;
    }

    void OnTriggerEnter2D(Collider2D collision) //Funkcia, ktorá sa vykoná pri kolízií hráča s objektom
    {
        switch (type) //1:glock, 2:AK, 101:First Aid Kit
        {
            //////////////////////////
            //        ZBRANE        //
            //        1-100         //
            //////////////////////////
            case 1:
                if (collision.gameObject.tag == "Player") //Overenie, či sa jedná o objekt s tagom "Player", čiže hráča
                {
                    collision.gameObject.GetComponent<Player>().GiveAmmo(type, amount); //Dá hráčovi náboje
                    if (!collision.gameObject.GetComponent<Player>().HasWeapon(type)) GiveItem(collision.gameObject, type); //Ak nemá zbraň, tak mu ju dá
                    string str = "Picked: " + amount + " 9mm bullets"; //Naformátuje string
                    collision.gameObject.GetComponent<Sounds>().PlaySound(4, collision.gameObject.GetComponent<AudioSource>()); //Prehrá zvuk
                    ShowText(str, 1.5f, 0); //Zobrazí text v hre
                    Destroy(gameObject); //Zničí sa
                }
                break;
            case 2:
                if (collision.gameObject.tag == "Player") //Overenie, či sa jedná o objekt s tagom "Player", čiže hráča
                {
                    collision.gameObject.GetComponent<Player>().GiveAmmo(type, amount); //Dá hráčovi náboje
                    if (!collision.gameObject.GetComponent<Player>().HasWeapon(type)) GiveItem(collision.gameObject, type); //Ak nemá zbraň, tak mu ju dá
                    string str = "Picked: " + amount + " 7.62×39mm bullets"; //Naformátuje string
                    collision.gameObject.GetComponent<Sounds>().PlaySound(4, collision.gameObject.GetComponent<AudioSource>()); //Prehrá zvuk
                    ShowText(str, 1.5f, 0); //Zobrazí text v hre
                    Destroy(gameObject); //Zničí sa
                }
                break;
            ///////////////////////
            //       ITEMY       //
            //       101+        //
            ///////////////////////
            case 101: //Lékarnička
                if (collision.gameObject.tag == "Player") //Overenie, či sa jedná o objekt s tagom "Player", čiže hráča
                {
                    if(collision.gameObject.GetComponent<Player>().GetHealth() < 100)
                    {
                        ShowText("Healed", 1.5f, 0);
                        collision.gameObject.GetComponent<Player>().SetHealth(100);
                        collision.gameObject.GetComponent<Sounds>().PlaySound(4, collision.gameObject.GetComponent<AudioSource>()); //Prehrá zvuk
                        Destroy(gameObject);
                    }
                }
                break;
            case 102: //Vesta
                if (collision.gameObject.tag == "Player") //Overenie, či sa jedná o objekt s tagom "Player", čiže hráča
                {
                    if(collision.gameObject.GetComponent<Player>().GetArmor() < 100)
                    {
                        ShowText("Armor replenished", 1.5f, 0);
                        collision.gameObject.GetComponent<Player>().SetArmor(100);
                        collision.gameObject.GetComponent<Sounds>().PlaySound(4, collision.gameObject.GetComponent<AudioSource>()); //Prehrá zvuk
                        Destroy(gameObject);
                    }
                }
                break;
        }
    }

    private void ShowText(string str, float offset, int color) //Funkcia, ktorá slúži na zobrazenie textu v hre
    {
        GameObject text = Instantiate(floatingText, gameObject.transform.position, Quaternion.identity); //Vytvorí objekt (Text)
        text.GetComponent<FloatingScript>().SetText(str, offset, color); //Nastaví mu text, odchylku a farbu
    }

    private void GiveItem(GameObject obj, int type)
    {
        string str;
        switch (type)
        {
            case 1:
                str = "Picked: Glock-21"; //Formátovanie textu
                obj.GetComponent<Player>().SetWeapon(type, true); //Nastavenie zbrane
                ShowText(str, 2f, 0); //Zobrazenie textu
                break;
            case 2:
                str = "Picked: Ak-47"; //Formátovanie textu
                obj.GetComponent<Player>().SetWeapon(type, true); //Nastavenie zbrane
                ShowText(str, 2f, 0); //Zobrazenie textu
                break;
        }
    }
}
