using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : ItemSelection, IShopUI
{
    [SerializeField] private DialogBox dialogBox;
    [SerializeField] private ShopItemScrollSelection itemSelection;
    [SerializeField] private TextCycleSelection cycleSelection;
    [SerializeField] private ShadowedText moneyText;
    [SerializeField] private ShadowedText inBagCountText;
    [SerializeField] private AudioClip buySound;

    public ShopUI() => Services.Register(this as IShopUI);

    private ItemData chosenItem;

    private void RefreshMoney() => moneyText.text = $"\n{Money.FormatMoneyToString(PlayerData.Instance.money)}";
    private void RefreshInBagCount(ItemData item) => inBagCountText.text = $"\nx  {PlayerData.Instance.GetItemCount(item)}";

    public void Open(Action<ISelectableUIElement, bool> callback, IItemData[] items)
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
    private void ChooseItem(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
        {
            callback?.Invoke(null, true);
            return;
        }

        chosenItem = ((ItemShopListEntryUI)selection).item;
        dialogBox.DrawText($"{chosenItem.Name}? Aber gerne.\nWie ?", DialogBoxContinueMode.External);
        cycleSelection.Open(ChooseQuantity, chosenItem.Price);
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
