using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;

[Serializable]
public class PlayerData : CharacterData
{
    static public PlayerData Instance;

    public PlayerData() => Instance = this;

    public float money = 0;
    public Dictionary<ItemCategory, List<Item>> items = new Dictionary<ItemCategory, List<Item>>
    {
        { ItemCategory.Food, new List<Item>() },
        { ItemCategory.Items, new List<Item>() },
        { ItemCategory.KeyItems, new List<Item>() },
        { ItemCategory.Medicine, new List<Item>() },
        { ItemCategory.TMs, new List<Item>() },
    };
    public Transform lastPokeCenterEntrance;

    [SerializeField] private List<Item> itemsValues;

    public void LoadDefault()
    {
        FillItemsDict(itemsValues);
        InitCaughtPokemon();
    }

    public void FillItemsDict(List<Item> itemsList)
    {
        foreach (Item item in itemsList)
            GiveItem(item);
    }

    public HashSet<PokemonData> seenPokemons = new HashSet<PokemonData>();
    public HashSet<PokemonData> caughtPokemon = new HashSet<PokemonData>();
    public List<Pokemon> pokemonsInBox = new List<Pokemon>();

    public void PutPokemonToBox(Pokemon pokemon) => pokemonsInBox.Add(pokemon);

    public void MovePokemonFromPartyToBox(Pokemon pokemon)
    {
        pokemonsInBox.Add(pokemon);
        pokemons.Remove(pokemon);
    }

    public void MovePokemonFromBoxToParty(Pokemon pokemon)
    {
        pokemons.Add(pokemon);
        pokemonsInBox.Remove(pokemon);
    }

    public void SwapPartyToBox(Pokemon partyPokemon, Pokemon boxPokemon)
    {
        int partyIndex = pokemons.IndexOf(partyPokemon);
        int boxIndex = pokemonsInBox.IndexOf(boxPokemon);
        pokemons[partyIndex] = boxPokemon;
        pokemonsInBox[boxIndex] = partyPokemon;
    }

    public void AddCaughtPokemon(PokemonData pokemon)
    {
        caughtPokemon.Add(pokemon);
        AddSeenPokemon(pokemon);
    }

    public void AddSeenPokemon(PokemonData pokemon) => seenPokemons.Add(pokemon);
    public bool HasCaughtPokemon(PokemonData pokemon) => caughtPokemon.Contains(pokemon);
    public bool HasSeenPokemon(PokemonData pokemon) => seenPokemons.Contains(pokemon);

    public void CatchPokemon(Pokemon pokemon)
    {
        GivePokemon(pokemon);
        pokemon.metDate = DateTime.Now;
        pokemon.metLevel = pokemon.level.ToString();
        // TODO: Enter actual map
        pokemon.metMap = "Dortmund";
        AddCaughtPokemon(pokemon.data);
    }

    public override void GivePokemon(Pokemon pokemon)
    {
        if (PartyIsFull())
            pokemonsInBox.Add(pokemon);
        else
            base.GivePokemon(pokemon);
    }

    public override float GetPriceMoney() => Mathf.Clamp(0.05f * money, 0, 50000f);
    public void GiveMoney(float amount) => money += amount;
    public float TakeMoney(float amount)
    {
        float taken = Mathf.Min(money, amount);
        money = Mathf.Max(0, money - taken); ;
        return taken;
    }

    public bool TryTakeMoney(float amount)
    {
        if (amount > money)
            return false;

        TakeMoney(amount);
        return true;
    }

    public void GiveItem(Item item)
    {
        List<Item> list = items[item.data.category];
        Item presentItem = list.Find(x => x.Equals(item));
        if (item.data.stacks)
            if (presentItem is null)
                list.Add(item);
            else
                presentItem.Increase(item.Count);
        else
            for (int i = 0; i < item.Count; i++)
                list.Add(item);
    }

    public bool TryTakeItem(Item item)
    {
        List<Item> list = items[item.data.category];
        Item presentItem = list.Find(x => x.Equals(item));

        if (presentItem is null)
            return false;

        if (item.data.stacks)
            if (presentItem.Decrease())
                return true;

        list.Remove(presentItem);
        return true;
    }

    public int GetItemCount(ItemData itemData)
    {
        List<Item> list = items[itemData.category];

        if (itemData.stacks)
        {
            Item presentItem = list.Find(x => x.data == itemData);
            return presentItem is null ? 0 : presentItem.Count;
        }

        return list.FindAll(x => x.data == itemData).Count;
    }

    public void SwapItems(Item item1, Item item2)
    {
        if (item1 == item2 || item1.data.category != item2.data.category)
            return;
        List<Item> bin = items[item1.data.category];
        int newIndex1 = items[item1.data.category].IndexOf(item2);
        int newIndex2 = items[item1.data.category].IndexOf(item1);
        bin[newIndex1] = item1;
        bin[newIndex2] = item2;
    }

    private void InitCaughtPokemon()
    {
        InitCaughtPokemonFromPokemons();
        InitCaughtPokemonsFromBox();
    }

    private void InitCaughtPokemonFromPokemons()
    {
        foreach (Pokemon p in pokemonsInBox)
            AddCaughtPokemon(p.data);
    }

    private void InitCaughtPokemonsFromBox()
    {
        foreach (Pokemon p in pokemonsInBox)
            AddCaughtPokemon(p.data);
    }

    public JSONArray ItemsToJSON()
    {
        JSONArray json = new JSONArray();

        foreach(List<Item> itemList in items.Values)
            foreach(Item item in itemList)
                json.Add(item.ToJSON());

        return json;
    }

    public JSONArray PokemonsToJSON()
    {
        JSONArray json = new JSONArray();

        foreach (Pokemon pokemon in pokemons)
            json.Add(pokemon.ToJSON());

        return json;
    }

    public JSONArray PokemonsInBoxToJSON()
    {
        JSONArray json = new JSONArray();

        foreach (Pokemon pokemon in pokemonsInBox)
            json.Add(pokemon.ToJSON());

        return json;
    }

    public JSONArray SeenPokemonsToJSON()
    {
        JSONArray json = new JSONArray();

        foreach (PokemonData pokemon in seenPokemons)
            json.Add(pokemon.Id);

        return json;
    }

    public void LoadItemsFromJSON(JSONArray json)
    {
        List<Item> itemsList = new List<Item>();

        foreach (JSONNode itemJSON in json)
            itemsList.Add(new Item(itemJSON));
        FillItemsDict(itemsList);
    }

    public void LoadPokemonsFromJSON(JSONArray json)
    {
        pokemons = new List<Pokemon>();
        foreach (JSONNode pokemonJSON in json)
            GivePokemon(new Pokemon(pokemonJSON, this));

        InitCaughtPokemonFromPokemons();
    }

    public void LoadPokemonsInBoxFromJSON(JSONArray json)
    {
        pokemonsInBox = new List<Pokemon>();
        foreach (JSONNode pokemonJSON in json)
            PutPokemonToBox(new Pokemon(pokemonJSON, this));

        InitCaughtPokemonsFromBox();
    }

    public void LoadSeenPokemonsFromJSON(JSONArray json)
    {
        foreach (JSONNode pokemonJSON in json)
            AddSeenPokemon((PokemonData)BaseScriptableObject.Get(pokemonJSON));
    }
}
