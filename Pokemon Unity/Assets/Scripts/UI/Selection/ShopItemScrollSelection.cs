using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemScrollSelection : ItemScrollSelection
{
    public ItemData selectedItem => (ItemData)selectedElement.GetPayload();

    protected override void SelectElement(int index)
    {
        base.SelectElement(index);
        description.text = selectedItem is null ? "" : selectedItem.Description;
    }
}
