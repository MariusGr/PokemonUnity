using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class CharacterData
{
    public string name;
    public string nameGenitive => name.Length > 0 && name[name.Length - 1] == 's' ? $"{name}'" : $"{name}s";
    public Pokemon[] pokemons;
    public GameObject gameobject;

    public void HealAllPokemons()
    {
        foreach (Pokemon p in pokemons)
            p.Heal();
    }

    public bool IsDefeated()
    {
        foreach (Pokemon p in pokemons)
            if (!p.isFainted)
                return false;
        return true;
    }

    abstract public float GetPriceMoney();
    public string GetPriceMoneyFormatted() => Money.FormatMoneyToString(GetPriceMoney());

    public int GetFirstAlivePokemonIndex()
    {
        for (int i = 0; i < pokemons.Length; i++)
        {
            if (!pokemons[i].isFainted)
                return i;
        }

        return -1;
    }
}
