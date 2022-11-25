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
    public Dictionary<ItemCategory, List<Item>> items;
    public Transform lastPokeCenterEntrance;

#if (UNITY_EDITOR)
    [SerializeField] private List<ItemCategory> itemsKeys = new List<ItemCategory>()
    {
        ItemCategory.Food,
        ItemCategory.Items,
        ItemCategory.KeyItems,
        ItemCategory.Medicine,
        ItemCategory.TMs,
    };
    [SerializeField] private List<Item> itemsValues;

    public void FillItemsDict()
    {

        foreach(Item item in itemsValues)

    }
#endif

    public override float GetPriceMoney() => Mathf.Clamp(0.05f * money, 0, 50000f);

    public void GiveMoney(float amount) => money += amount;
    public float TakeMoney(float amount)
    {
        float taken = Mathf.Min(money, amount);
        money = Mathf.Max(0, money - taken); ;
        return taken;
    }

    public void GiveItem(Item item)
    {
        List<Item> list = items[item.data.category];
        Item presentItem = list.Find(x => x.Equals(item));
        if (item.data.stacks && !(presentItem is null))
            presentItem.Increase(item.count);
        else
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
}
