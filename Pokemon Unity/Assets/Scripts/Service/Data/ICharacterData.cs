using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterData
{
    public string Name { get; }
    public string NameGenitive { get; }
    public List<IPokemon> Pokemons { get; }

    public void GivePokemon(IPokemon pokemon);
    public void SwapPokemons(IPokemon pokemon1, IPokemon pokemon2);
    public bool PartyIsFull();
    public bool WouldBeDeafeatetWithoutPokemon(IPokemon pokemon);
    public bool IsDefeated();
    public float GetPriceMoney();
    public string GetPriceMoneyFormatted();
    public int GetFirstAlivePokemonIndex();
    public void ExchangePokemon(IPokemon before, IPokemon after);
    public void HealAllPokemons();
}
