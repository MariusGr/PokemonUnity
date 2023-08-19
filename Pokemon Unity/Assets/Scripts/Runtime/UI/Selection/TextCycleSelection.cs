using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextCycleSelection : ScalarSelection
{
    [SerializeField] private ShadowedText text;

    private SelectableItemQuantity selectedQuantity => (SelectableItemQuantity)selectedElement;

    public void Open(Action<SelectableUIElement, bool> callback, float price)
    {
        AssignElements(price);
        base.Open(callback);
    }

    public void AssignElements(float price) {
        elements = new SelectableItemQuantity[10];
        object[] pricePayloads = new object[10];

        for (int i = 0; i < 10; i++)
        {
            elements[i] = new SelectableItemQuantity();
            pricePayloads[i] = price;
        }

        DebugExtensions.DebugExtension.Log(pricePayloads);
        base.AssignElements(pricePayloads);
    }

    protected override void SelectElement(int index, bool playSound)
    {
        base.SelectElement(index, playSound);
        int count = selectedIndex + 1;
        text.text = $"x  {count.ToString("00")}\n{selectedQuantity.GetTotalPriceString()}";
    }
}
