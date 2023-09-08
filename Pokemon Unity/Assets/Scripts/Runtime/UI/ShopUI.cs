using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : ItemSelection
{
    public static ShopUI Instance;

    [SerializeField] private DialogBox dialogBox;
    [SerializeField] private ShopItemScrollSelection itemSelection;
    [SerializeField] private TextCycleSelection cycleSelection;
    [SerializeField] private ShadowedText moneyText;
    [SerializeField] private ShadowedText inBagCountText;
    [SerializeField] private AudioClip buySound;

    public ShopUI() => Instance = this;

    private ItemData chosenItem;

    private void RefreshMoney() => moneyText.text = $"\n{Money.FormatMoneyToString(PlayerData.Instance.money)}";
    private void RefreshInBagCount(ItemData item) => inBagCountText.text = $"\nx  {PlayerData.Instance.GetItemCount(item)}";

    public void Open(Action<SelectableUIElement, bool> callback, ItemData[] items)
    {
        Open(callback);
        RefreshMoney();
        itemSelection.AssignItems(items);
        itemSelection.Open(ChooseItem, SelectItem);
    }

    public override void Close()
    {
        cycleSelection.Close();
        itemSelection.Close();
        base.Close();
    }

    private void SelectItem(ItemData item) => RefreshInBagCount(item);
    private void ChooseItem(SelectableUIElement selection, bool goBack)
    {
        if (goBack)
        {
            callback?.Invoke(null, true);
            return;
        }

        chosenItem = ((ItemShopListEntryUI)selection).item;
        dialogBox.DrawText($"{chosenItem.fullName}? Aber gerne.\nWie viele dürfen es sein?", DialogBoxContinueMode.External);
        cycleSelection.Open(ChooseQuantity, chosenItem.price);
    }

    private void ChooseQuantity(SelectableUIElement selection, bool goBack)
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
        cycleSelection.Close();

        if (!PlayerData.Instance.TryTakeMoney(quantity.GetTotalPrice()))
        {
            yield return dialogBox.DrawText(
                $"Du hast nicht genug Geld!\nDu brauchst noch {Money.FormatMoneyToString(quantity.GetTotalPrice() - PlayerData.Instance.money)}",
                DialogBoxContinueMode.User, closeAfterFinish: true);
        }
        
        else
        {
            PlayerData.Instance.GiveItem(new Item(chosenItem, quantity.count));
            RefreshInBagCount(chosenItem);
            RefreshMoney();
            SfxHandler.Play(buySound);
        }
    }
}
