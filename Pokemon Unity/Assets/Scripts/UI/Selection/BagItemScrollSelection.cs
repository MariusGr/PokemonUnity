using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagItemScrollSelection : ScrollSelection
{
    public Item choosenItem { get; private set; }
    private ItemListEntryUI choosenItemEntry;
    public bool itemHasBeenChoosen => !(choosenItemEntry is null);

    protected override void MoveViewFrame(int shift)
    {
        base.MoveViewFrame(shift);

        if (!itemHasBeenChoosen)
            return;

        choosenItemEntry.SetPlaceMode(false);
        int newPlaceIndex = choosenItemEntry.index - shift;

        if (newPlaceIndex >= 0 && newPlaceIndex < slots)
        {
            choosenItemEntry = (ItemListEntryUI)elements[newPlaceIndex];
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

    public bool ChooseItemEntry(ItemListEntryUI entry)
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
