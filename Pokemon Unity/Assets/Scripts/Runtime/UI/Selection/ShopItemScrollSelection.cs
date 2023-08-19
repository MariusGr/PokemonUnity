using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemScrollSelection : ItemScrollSelection
{
    public ItemData selectedItem => (ItemData)selectedElement.GetPayload();

    private Action<ItemData> selectedCallback;

    public void Open(Action<SelectableUIElement, bool> callback, Action<ItemData> selectedCallback)
    {
        this.selectedCallback = selectedCallback;
        base.Open(callback);
    }

    protected override void SelectElement(int index, bool playSound)
    {
        base.SelectElement(index, playSound);
        description.text = selectedItem is null ? "" : selectedItem.Description;
        selectedCallback?.Invoke(selectedItem);
    }
}
