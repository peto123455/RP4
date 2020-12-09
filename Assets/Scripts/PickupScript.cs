using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    [SerializeField] private  GameObject floatingText;
    [SerializeField] private Sprite glockSprite, akSprite, fakSprite, vestSprite, spasSprite;
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
        else if (type == 3) GetComponent<SpriteRenderer>().sprite = spasSprite;
        else if (type == 101) GetComponent<SpriteRenderer>().sprite = fakSprite;
        else if (type == 102) GetComponent<SpriteRenderer>().sprite = vestSprite;
    }

    void OnTriggerEnter2D(Collider2D collision) //Funkcia, ktorá sa vykoná pri kolízií hráča s objektom
    {
        if(type < 100)
        {
            if(collision.gameObject.tag == "Player")
            {
                GameObject Player = collision.gameObject;
                WeaponList.Weapon weapon = Player.GetComponent<Player>().GetWeaponByID(type); 

                weapon.GiveAmmo(amount); //Dá hráčovi náboje
                if (!weapon.HasWeapon()) GiveWeapon(weapon); //Ak nemá zbraň, tak mu ju dá
                string str = "Picked: " + amount; //Naformátuje string
                switch(weapon.bulletType)
                {
                    case 1:
                        str += " 9mm bullets";
                        break;
                    case 2:
                        str += " 7.62×39mm bullets";
                        break;
                    case 3:
                        str += " shotgun slug rounds";
                        break;
                }
                Player.GetComponent<Sounds>().PlaySound(4, Player.GetComponent<AudioSource>()); //Prehrá zvuk
                ShowText(str, 1.5f, 0); //Zobrazí text v hre
                Destroy(gameObject); //Zničí sa
            }
        }
        else
        switch (type) //1:glock, 2:AK, 101:First Aid Kit
        {
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

    private void GiveWeapon(WeaponList.Weapon weapon)
    {
        string str = "Picked: " + weapon.name;
        weapon.SetWeapon(true); //Nastavenie zbrane
        ShowText(str, 2f, 0); //Zobrazenie textu
    }
}
