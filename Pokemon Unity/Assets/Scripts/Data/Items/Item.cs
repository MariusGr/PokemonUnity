using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;

[Serializable]
public class Item
{
    public ItemData data;
    [SerializeField] private int count = 1;
    public int Count => count;

    public Item(ItemData data, int count)
    {
        this.data = data;
        this.count = count;
    }

    public Item(JSONNode json)
    {
        data = (ItemData)BaseScriptableObject.Get(json["data"]);
        count = json["count"];
    }

    public void Increase(int amount = 1)
    {
        count += amount;
        Debug.Log("Item Count  " + count);
    }

    public bool Decrease(int amount = 1)
    {
        count = Math.Max(0, count - amount);
        return count > 0;
    }

    public override bool Equals(object other)
    {
        if (data.stacks)
            return data == ((Item)other).data;
        else
            return base.Equals(other);
    }

    public override int GetHashCode()
    {
        if (data.stacks)
            return data.GetHashCode();
        else
            return base.GetHashCode();
    }

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();
        json.Add("data", data.Id);
        json.Add("count", count);
        return json;
    }

    public override string ToString() => $"{base.ToString()}: {data} x{count}";
}
