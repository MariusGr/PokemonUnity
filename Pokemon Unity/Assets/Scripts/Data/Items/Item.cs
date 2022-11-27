using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Item
{
    public ItemData data;
    public int count { get; private set; } = 1;
    public string Description => data.moveLearned is null ? data.description : data.moveLearned.description;

    public void Increase(int amount = 1) => count += amount;
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

    public override string ToString()
    {
        return $"{base.ToString()}: {data} x{count}";
    }
}
