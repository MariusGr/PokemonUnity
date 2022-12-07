using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : ItemSelection, IShopUI
{
    [SerializeField] private DialogBox dialogBox;
    [SerializeField] private BagItemScrollSelection itemSelection;
    [SerializeField] private TextCycleSelection cycleSelection;

    public ShopUI() => Services.Register(this as IShopUI);

    public override void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        base.Open(callback, forceSelection, startSelection);
        itemSelection.Open(ChooseItem);
    }

    private void ChooseItem(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
        {
            callback?.Invoke(null, true);
            return;
        }

        StartCoroutine(ChooseItemCoroutine(selection));
    }

    private IEnumerator ChooseItemCoroutine(ISelectableUIElement selection)
    {
        ItemData item = ((ItemShopListEntryUI)selection).item;

        yield return dialogBox.DrawText($"{item.fullName}?Aber gerne.\nWie viele sollen's sein?", DialogBoxContinueMode.External);
        // TODO: Item Datas assignen, welche Items hat der Shop?
        cycleSelection.Open(???);
    }
}
