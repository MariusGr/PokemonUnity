using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CharacterData
{
    public string name;
    public string nameGenitive => name.Length > 0 && name[name.Length - 1] == 's' ? $"{name}'" : $"{name}s";
    public float priceMoneyBase = 0;
    public Pokemon[] pokemons;

    public bool IsDefeated()
    {
        foreach (Pokemon p in pokemons)
            if (!p.isFainted)
                return false;
        return true;
    }

    public float GetPriceMoney() => pokemons[pokemons.Length - 1].level * priceMoneyBase;
    public string GetPriceMoneyFormatted() => Money.FormatMoneyToString(GetPriceMoney());
}
