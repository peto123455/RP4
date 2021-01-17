using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private Sounds sounds;

    [SerializeField] Text   buttonText, moneyText, healthPriceText, shieldPriceText, fovPriceText, rcPriceText, laserPriceText;
    [SerializeField] Slider healthSlider, shieldSlider, fovSlider;
    [SerializeField] Button 
    rcButton, rcButtonBuy,
    laserButton, laserButtonBuy;

    [SerializeField] GameObject tab1, tab2;

    int money = 0;

    List<ShopItem> items = new List<ShopItem>();
    List<GadgetShopItem> gadgets = new List<GadgetShopItem>();

    GadgetShopItem selected = null;

    void Awake()
    {
        sounds = new Sounds();
        WriteList();
        Reset();
    }

    private void WriteList()
    {
        items.Clear();
        items.Add(new ShopItem("healthLvl", new int[] {10,20,50,100,200}, healthPriceText, healthSlider)); //Health
        items.Add(new ShopItem("shieldLvl", new int[] {10,20,50,100,200}, shieldPriceText, shieldSlider)); //Shield
        items.Add(new ShopItem("fovLvl", new int[] {10,20,30,50,100}, fovPriceText, fovSlider)); //FOV

        gadgets.Clear();
        gadgets.Add(new GadgetShopItem(0, "rc", 1000, rcPriceText, rcButton, rcButtonBuy));
        gadgets.Add(new GadgetShopItem(1, "laser", 350, laserPriceText, laserButton, laserButtonBuy));
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

        foreach(GadgetShopItem gadgetShopItem in gadgets)
        {
            gadgetShopItem.buttonBuy.interactable = !gadgetShopItem.bought;
            gadgetShopItem.button.interactable = gadgetShopItem.bought;
            gadgetShopItem.priceText.text = gadgetShopItem.price.ToString() + " $";
        }
    }

    public void Reset()
    {
        money = PlayerPrefs.GetInt("money", 0);
        Select(PlayerPrefs.GetInt("selectedGadget", -1));
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

    public void Buy(int id)
    {
        GadgetShopItem gadgetShopItem = IdentifyGadget(id);
        if(gadgetShopItem.price > money) return;

        money -= gadgetShopItem.price;
        gadgetShopItem.bought = true;
        Refresh();
    }

    public void Select(int id)
    {
        if(id == -1)
        {
            selected = null;
            return;
        }

        GadgetShopItem gadgetShopItem = IdentifyGadget(id);
        if(!gadgetShopItem.bought) return;

        selected = gadgetShopItem;
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

    public GadgetShopItem IdentifyGadget(int id)
    {
        return gadgets[id];
    }

    private void LoadItems()
    {
        foreach(ShopItem shopItem in items)
        {
            shopItem.before = PlayerPrefs.GetInt(shopItem.name, 0);
            shopItem.current = shopItem.before;
        }

        foreach(GadgetShopItem gadgetShopItem in gadgets)
        {
            gadgetShopItem.bought = MathFunctions.IntToBool(PlayerPrefs.GetInt(gadgetShopItem.name, 0));
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

        foreach(GadgetShopItem gadgetShopItem in gadgets)
        {
            PlayerPrefs.SetInt(gadgetShopItem.name, MathFunctions.BoolToInt(gadgetShopItem.bought));
        }

        if(selected != null) PlayerPrefs.SetInt("selectedGadget", selected.id);
        PlayerPrefs.SetInt("money", money);
    }

    public void SwitchTab()
    {
        if(tab1.activeSelf)
        {
            tab1.SetActive(false);
            tab2.SetActive(true);
            buttonText.text = "Upgrades";
        }
        else
        {
            tab1.SetActive(true);
            tab2.SetActive(false);
            buttonText.text = "Gadgets";
        }
    }
}
