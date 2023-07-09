using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;

public interface IItem : IJSONConvertable
{
    public InterfaceReference<IItemData, ScriptableObject> Data { get; }
    public int Count { get; }

    public void Increase(int amount = 1);
    public bool Decrease(int amount = 1);
}
