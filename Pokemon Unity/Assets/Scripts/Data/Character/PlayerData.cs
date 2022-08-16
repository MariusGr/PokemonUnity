using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class PlayerData : CharacterData
{
    public float money = 0;

    public override float GetPriceMoney() => Mathf.Clamp(0.05f * money, 0, 50000f);

    public void GiveMoney(float amount) => money += amount;
    public float TakeMoney(float amount)
    {
        float taken = Mathf.Min(money, amount);
        money = Mathf.Max(0, money - taken); ;
        return taken;
    }
}
