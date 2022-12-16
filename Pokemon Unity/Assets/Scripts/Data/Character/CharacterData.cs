using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public abstract class CharacterData
{
    public string name;
    public string nameGenitive => name.Length > 0 && name[name.Length - 1] == 's' ? $"{name}'" : $"{name}s";
    public List<Pokemon> pokemons;
    public GameObject gameobject;

    public void HealAllPokemons()
    {
        foreach (Pokemon p in pokemons)
            p.HealFully();
    }

    public virtual void GivePokemon(Pokemon pokemon)
    {
        if (pokemons.Contains(pokemon))
        {
            Debug.LogWarning($"Tried to give {this} on Gameobject {gameobject} pokemon {pokemon.data.name} but pokemon ist already in party.");
            return;
        }
        if (pokemons.Count > 5)
        {
            Debug.LogWarning($"Tried to give {this} on Gameobject {gameobject} pokemon {pokemon.data.name} but party is full.");
            return;
        }
        pokemons.Add(pokemon);
    }

    public void SwapPokemons(Pokemon pokemon1, Pokemon pokemon2)
    {
        if (pokemon1 == pokemon2)
            return;
        int newIndex1 = pokemons.IndexOf(pokemon2);
        int newIndex2 = pokemons.IndexOf(pokemon1);
        pokemons[newIndex1] = pokemon1;
        pokemons[newIndex2] = pokemon2;
    }

    public bool PartyIsFull() => pokemons.Count > 5;
    public bool WouldBeDeafeatetWithoutPokemon(Pokemon pokemon) => pokemons.Count(p => !p.isFainted && p != pokemon) < 1;

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
        for (int i = 0; i < pokemons.Count; i++)
        {
            if (!pokemons[i].isFainted)
                return i;
        }

        return -1;
    }

    public void ExchangePokemon(Pokemon before, Pokemon after)
    {
        int index = pokemons.IndexOf(before);
        pokemons[index] = after;
    }
}
