using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextCycleSelection : ScalarSelection
{
    [SerializeField] private ShadowedText text;

    private SelectableItemQuantity selectedQuantity => (SelectableItemQuantity)selectedElement;

    public void Open(Action<ISelectableUIElement, bool> callback, float price)
    {
        base.Open(callback);
        AssignElements(price);
    }

    public void AssignElements(float price) {
        SelectableItemQuantity[] items = new SelectableItemQuantity[10];
        for (int i = 0; i < 10; i++)
            items[i] = new SelectableItemQuantity();
        AssignElements(items);
    }

    protected override void SelectElement(int index)
    {
        base.SelectElement(index);
        int count = selectedIndex + 1;
        text.text = $"{count.ToString("00")}\n{selectedQuantity.GetTotalPriceString()}";
    }
}
