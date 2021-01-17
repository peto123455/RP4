using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GadgetShopItem
{
    public string name;
    public int price, id;
    public Text priceText;
    public bool bought = false;

    public Button button, buttonBuy;

    public GadgetShopItem(int id, string name, int price, Text priceText, Button button, Button buttonBuy)
    {
        this.id = id;
        this.name = name;
        this.price = price;
        this.priceText = priceText;
        this.button = button;
        this.buttonBuy = buttonBuy;
    }
}
