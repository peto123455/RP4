using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem
{
    public string name;
    public Text priceText;
    public Slider slider;
    public int before, current;
    public int[] prices = {0,0,0,0,0}; 

    //public ShopItem(int price1, int price2, int price3, int price4, int price5)
    public ShopItem(string name, int[] prices, Text priceText, Slider slider)
    {
        this.name = name;
        this.prices = prices;
        this.priceText = priceText;
        this.slider = slider;
    }
}
