using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CollectionExtensions;
using System.Linq;


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

#if (UNITY_EDITOR)
    [SerializeField] private List<Item> itemsValues;

    public void FillItemsDict()
    {
        foreach (Item item in itemsValues)
            GiveItem(item);
    }
#endif

    public HashSet<PokemonData> seenPokemon = new HashSet<PokemonData>();
    public HashSet<PokemonData> caughtPokemon = new HashSet<PokemonData>();

    public void AddCaughtPokemon(PokemonData pokemon)
    {
        caughtPokemon.Add(pokemon);
        AddSeenPokemon(pokemon);
    }

    public void AddSeenPokemon(PokemonData pokemon) => seenPokemon.Add(pokemon);
    public bool HasCaughtPokemon(PokemonData pokemon) => caughtPokemon.Contains(pokemon);
    public bool HasSeenPokemon(PokemonData pokemon) => seenPokemon.Contains(pokemon);

    public override void GivePokemon(Pokemon pokemon)
    {
        base.GivePokemon(pokemon);
        pokemon.metDate = DateTime.Now;
        pokemon.metLevel = pokemon.level.ToString();
        // TODO: Enter actual map
        pokemon.metMap = "Dortmund";
        AddCaughtPokemon(pokemon.data);
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
}
