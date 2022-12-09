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

    private ItemData chosenItem;

    public void Open(Action<ISelectableUIElement, bool> callback, ItemData[] items)
    {
        Open(callback);
        itemSelection.AssignElements(items);
        itemSelection.Open(ChooseItem);
    }

    private void ChooseItem(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
        {
            callback?.Invoke(null, true);
            return;
        }

        chosenItem = ((ItemShopListEntryUI)selection).item;
        dialogBox.DrawText($"{chosenItem.fullName}?Aber gerne.\nWie viele sollen's sein?", DialogBoxContinueMode.External);
        cycleSelection.Open(ChooseQuantity);
    }

    private void ChooseQuantity(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
        {
            cycleSelection.Close();
            dialogBox.Close();
            return;
        }

        StartCoroutine(ChooseQuantityCoroutine((SelectableItemQuantity)selection));
    }

    private IEnumerator ChooseQuantityCoroutine(SelectableItemQuantity quantity)
    {
        if (PlayerData.Instance.TryTakeMoney(quantity.GetTotalPrice()))
            yield return dialogBox.DrawText(
                $"Du hast nicht genug Geld!\nDu brauchst noch {Money.FormatMoneyToString(quantity.GetTotalPrice() - PlayerData.Instance.money)}",
                DialogBoxContinueMode.User);
        else
        {
            PlayerData.Instance.GiveItem(new Item(chosenItem, quantity.count));
            cycleSelection.Close();
            //TODO sound
        }
    }
}
