using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money
{
    public int money = 0;

    public void GiveMoney(int money)
    {
        this.money += money;
    }

    public void SetMoney(int money)
    {
        this.money = money;
    }

    public void TakeMoney(int money)
    {
        this.money = money;
    }

    public int GetMoney()
    {
        return this.money;
    }
}
