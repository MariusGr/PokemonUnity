using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;

[Serializable]
public class Item : IItem
{
    [field: SerializeField] public IItemData data { get; private set; }
    [field: SerializeField] public int Count { get; private set; } = 0;

    public Item(ItemData data, int count)
    {
        this.data = data;
        Count = count;
    }

    public Item(JSONNode json)
    {
        data = (ItemData)BaseScriptableObject.Get(json["data"]);
        Count = json["count"];
    }

    public void Increase(int amount = 1)
    {
        Count += amount;
        Debug.Log("Item Count  " + Count);
    }

    public bool Decrease(int amount = 1)
    {
        Count = Math.Max(0, Count - amount);
        return Count > 0;
    }

    public override bool Equals(object other)
    {
        if (data.Stacks)
            return data == ((Item)other).data;
        else
            return base.Equals(other);
    }

    public override int GetHashCode()
    {
        if (data.Stacks)
            return data.GetHashCode();
        else
            return base.GetHashCode();
    }

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();
        json.Add("data", data.Id);
        json.Add("count", Count);
        return json;
    }

    public override string ToString() => $"{base.ToString()}: {data} x{Count}";
}
