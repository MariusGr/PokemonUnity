using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScrollSelection : ScalarSelection
{
    [SerializeField] private int slots = 6;
    private List<Item> items;

    public void AssignItems(List<Item> items)
    {
        DebugExtensions.DebugExtension.Log(items);
        this.items = items;
        AssignElements(this.items.Take(slots).ToArray());
    }
}
