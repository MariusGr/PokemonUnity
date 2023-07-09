using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;
using AYellowpaper;

[Serializable]
public class Item : IItem
{
    [field: SerializeField] public InterfaceReference<IItemData, ScriptableObject> Data { get; private set; }
    [field: SerializeField] public int Count { get; private set; } = 0;

    public Item(ItemData data, int count)
    {
        Data.Value = data;
        Count = count;
    }

    public Item(JSONNode json)
    {
        Data.Value = (ItemData)BaseScriptableObject.Get(json["data"]);
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
        if (Data.Value.Stacks)
            return Data == ((Item)other).Data;
        else
            return base.Equals(other);
    }

    public override int GetHashCode()
    {
        if (Data.Value.Stacks)
            return Data.GetHashCode();
        else
            return base.GetHashCode();
    }

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();
        json.Add("data", Data.Value.Id);
        json.Add("count", Count);
        return json;
    }

    public override string ToString() => $"{base.ToString()}: {Data} x{Count}";
}
