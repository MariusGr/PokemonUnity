using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableItemQuantity : ISelectableUIElement
{
    public bool IsAssigned() => true;
    public int GetIndex() => count - 1;
    public object GetPayload() => basePrice;
    public int count { get; private set; }
    public float basePrice { get; private set; }

    public float GetTotalPrice() => basePrice * count;
    public string GetTotalPriceString() => Money.FormatMoneyToString(GetTotalPrice());
    public ISelectableUIElement GetNeighbour(Direction direction) => null;
    public void Select() { }
    public void Deselect() { }
    public void Refresh() { }
    public void AssignElement(object payload) => basePrice = (float)payload;
    public void AssignNone() { }
    public void Initialize(int index) => this.count = index + 1;
}
