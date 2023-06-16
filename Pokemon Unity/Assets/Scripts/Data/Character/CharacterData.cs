using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public abstract class CharacterData : ICharacterData
{
    [field: SerializeField] public string Name { get; private set; }
    public string NameGenitive => Name.Length > 0 && Name[Name.Length - 1] == 's' ? $"{Name}'" : $"{Name}s";
    [field: SerializeField] public List<IPokemon> Pokemons { get; protected set; }
    [field: SerializeField] public GameObject Gameobject { get; private set; }

    public void HealAllPokemons()
    {
        foreach (Pokemon p in Pokemons)
            p.HealFully();
    }

    public virtual void GivePokemon(IPokemon pokemon)
    {
        if (Pokemons.Contains(pokemon))
        {
            Debug.LogWarning($"Tried to give {this} on Gameobject {Gameobject} pokemon {pokemon.Data.Name} but pokemon ist already in party.");
            return;
        }
        if (Pokemons.Count > 5)
        {
            Debug.LogWarning($"Tried to give {this} on Gameobject {Gameobject} pokemon {pokemon.Data.Name} but party is full.");
            return;
        }
        Pokemons.Add((Pokemon)pokemon);
    }

    public void SwapPokemons(IPokemon pokemon1, IPokemon pokemon2)
    {
        if (pokemon1 == pokemon2)
            return;

        Pokemon p1 = (Pokemon)pokemon1;
        Pokemon p2 = (Pokemon)pokemon2;
        int newIndex1 = Pokemons.IndexOf(p2);
        int newIndex2 = Pokemons.IndexOf(p1);
        Pokemons[newIndex1] = p1;
        Pokemons[newIndex2] = p2;
    }

    public bool PartyIsFull() => Pokemons.Count > 5;
    public bool WouldBeDeafeatetWithoutPokemon(IPokemon pokemon) => Pokemons.Count(p => !p.IsFainted && p != pokemon) < 1;

    public bool IsDefeated()
    {
        foreach (Pokemon p in Pokemons)
            if (!p.IsFainted)
                return false;
        return true;
    }

    abstract public float GetPriceMoney();
    public string GetPriceMoneyFormatted() => Money.FormatMoneyToString(GetPriceMoney());

    public int GetFirstAlivePokemonIndex()
    {
        for (int i = 0; i < Pokemons.Count; i++)
        {
            if (!Pokemons[i].IsFainted)
                return i;
        }

        return -1;
    }

    public void ExchangePokemon(IPokemon before, IPokemon after)
    {
        int index = Pokemons.IndexOf((Pokemon)before);
        Pokemons[index] = (Pokemon)after;
    }
}
