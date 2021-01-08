using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private Sounds sounds;

    [SerializeField] Text   moneyText, healthPriceText, shieldPriceText;
    [SerializeField] Slider healthSlider, shieldSlider;

    int money = 0;

    List<ShopItem> items = new List<ShopItem>();

    void Awake()
    {
        sounds = new Sounds();
        money = PlayerPrefs.GetInt("money", 0);
        WriteList();
        LoadItems();
        Refresh();
    }

    private void WriteList()
    {
        items.Clear();
        items.Add(new ShopItem("healthLvl", new int[] {10,20,50,100,200}, healthPriceText, healthSlider)); //Health
        items.Add(new ShopItem("shieldLvl", new int[] {10,20,50,100,200}, shieldPriceText, shieldSlider)); //Shield
    }

    private void Refresh()
    {
        this.moneyText.text = money.ToString() + " $";

        foreach(ShopItem shopItem in items)
        {
            shopItem.slider.value = shopItem.current * 20;
            if(shopItem.current <= 4) shopItem.priceText.text = shopItem.prices[shopItem.current].ToString() + " $";
            else shopItem.priceText.text = "MAX";
        }
    }

    public void Reset()
    {
        money = PlayerPrefs.GetInt("money", 0);
        LoadItems();
        Refresh();
    }

    public void Add(int id)
    {
        ShopItem shopItem = Identify(id);
        if(shopItem.current >= 5 || shopItem.prices[shopItem.current] > money) return;

        money -= shopItem.prices[shopItem.current];
        ++shopItem.current;
        Refresh();
    }

    public void Take(int id)
    {
        ShopItem shopItem = Identify(id);
        if(shopItem.current <= shopItem.before) return;

        money += shopItem.prices[shopItem.current - 1];
        --shopItem.current;
        Refresh();
    }

    public ShopItem Identify(int id)
    {
        return items[id];
    }

    private void LoadItems()
    {
        foreach(ShopItem shopItem in items)
        {
            shopItem.before = PlayerPrefs.GetInt(shopItem.name, 0);
            shopItem.current = shopItem.before;
        }
    }

    public void Apply()
    {
        Save();
        sounds.PlaySound(10, GameObject.Find("Player").GetComponent<AudioSource>());
        Reset();
    }

    private void Save()
    {
        foreach(ShopItem shopItem in items)
        {
            PlayerPrefs.SetInt(shopItem.name, shopItem.current);
        }

        PlayerPrefs.SetInt("money", money);
    }
}
