using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScrollSelection : ScalarSelection
{
    [SerializeField] private int slots = 6;
    [SerializeField] private ShadowedText description;

    public Item selectedItem => (Item)selectedElement.GetPayload();

    private List<Item> items;

    protected override void SelectElement(int index)
    {
        base.SelectElement(index);
        if (!(selectedItem is null))
            description.text = selectedItem.Description;
    }

    public void AssignItems(List<Item> items)
    {
        DebugExtensions.DebugExtension.Log(items);
        this.items = items;
        AssignElements(this.items.Take(slots).ToArray());
    }
}
