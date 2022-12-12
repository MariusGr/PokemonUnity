using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableItemQuantity : SelectableUIElement
{
    public int count { get; private set; }
    public float basePrice { get; private set; }

    public float GetTotalPrice() => basePrice * count;
    public string GetTotalPriceString() => Money.FormatMoneyToString(GetTotalPrice());
    public override ISelectableUIElement GetNeighbour(Direction direction) => null;
    public override void AssignElement(object payload)
    {
        basePrice = (float)payload;
        base.AssignElement(payload);
    }

    public override void Initialize(int index)
    {
        base.Initialize(index);
        count = index + 1;
    }
}
