using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagItemScrollSelection : ItemScrollSelection
{
    public Item choosenItem { get; private set; }
    public ItemBagListEntryUI choosenItemEntry { get; private set; }
    public bool itemHasBeenChoosen => !(choosenItemEntry is null);
    public Item selectedItem => (Item)selectedElement.GetPayload();

    protected override void SelectElement(int index)
    {
        base.SelectElement(index);
        description.text = selectedItem is null ? "" : selectedItem.data.Description;
    }

    protected override void MoveViewFrame(int shift)
    {
        base.MoveViewFrame(shift);

        if (!itemHasBeenChoosen)
            return;

        choosenItemEntry.SetPlaceMode(false);
        int newPlaceIndex = choosenItemEntry.index - shift;

        if (newPlaceIndex >= 0 && newPlaceIndex < slots)
        {
            choosenItemEntry = (ItemBagListEntryUI)_elements[newPlaceIndex];
            choosenItemEntry.SetPlaceMode(true);
        }
    }

    public void ResetItemSelection()
    {
        if (itemHasBeenChoosen)
        {
            choosenItemEntry.SetPlaceMode(false);
            choosenItemEntry = null;
            choosenItem = null;
        }
    }

    public bool ChooseItemEntry(ItemBagListEntryUI entry)
    {
        if (!itemHasBeenChoosen)
        {
            choosenItemEntry = entry;
            choosenItem = choosenItemEntry.item;
            choosenItemEntry.SetPlaceMode(true);
            return true;
        }

        PlayerData.Instance.SwapItems(choosenItem, entry.item);
        ResetItemSelection();
        return false;
    }
}
