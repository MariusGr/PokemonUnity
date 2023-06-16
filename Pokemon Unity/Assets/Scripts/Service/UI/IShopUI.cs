using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShopUI : ICallbackUIView
{
    public void Open(System.Action<ISelectableUIElement, bool> callback, IItemData[] items);
}
