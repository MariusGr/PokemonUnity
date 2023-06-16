using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem : IJSONConvertable
{
    public IItemData data { get; }
    public int Count { get; }

    public void Increase(int amount = 1);
    public bool Decrease(int amount = 1);
}
