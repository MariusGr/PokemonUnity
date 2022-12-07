using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextCycleSelection : ScalarSelection
{
    [SerializeField] private ShadowedText text;

    private float price;

    public void Open(Action<ISelectableUIElement, bool> callback, float price)
    {
        base.Open(callback);
        this.price = price;
    }

    public override void AssignElements() => AssignElements(new object[10]);
    public override void AssignElements(object[] elements) => base.AssignElements(new object[10]);

    protected override void SelectElement(int index)
    {
        base.SelectElement(index);
        int count = selectedIndex + 1;
        text.text = $"{count.ToString("00")}\n{Money.FormatMoneyToString(count * price)}";
    }
}
