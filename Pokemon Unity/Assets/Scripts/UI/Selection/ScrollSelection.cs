using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ScrollSelection : ScalarSelection
{
    [SerializeField] protected int slots = 6;
    [SerializeField] private ShadowedText description;

    public Item selectedItem => (Item)selectedElement.GetPayload();

    private List<Item> items;
    private int viewFrameStart = 0;
    private int shownItemsEnd => viewFrameStart + slots;
    private int scrollDownStart;
    private int scrollUpStart;

    public override void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        viewFrameStart = 0;
        base.Open(callback, forceSelection, startSelection);
    }

    protected override void SelectElement(int index)
    {
        if (items.Count > slots)
        {
            // scrolling is necessary since not all items fit into the view
            if (index > scrollDownStart && shownItemsEnd < items.Count)
            {
                // shown frame can be moved further down and selection will be far enough down
                MoveViewFrame(1);
                index = selectedIndex;
            } else if (index < scrollUpStart && viewFrameStart > 0)
            {
                // shown frame can be moved further down and selection will be far enough down
                MoveViewFrame(-1);
                index = selectedIndex;
            }
        }

        base.SelectElement(index);
        if (!(selectedItem is null))
            description.text = selectedItem.Description;
    }

    protected virtual void MoveViewFrame(int shift)
    {
        viewFrameStart = viewFrameStart + shift;
        RefreshViewFrame();
    }
    protected void RefreshViewFrame() => AssignElements(items.Skip(viewFrameStart).Take(slots).ToArray());

    public void AssignItems(List<Item> items)
    {
        if (!(this.items is null) && this.items.Count > items.Count)
            viewFrameStart = Math.Max(0, viewFrameStart - (this.items.Count - items.Count));

        this.items = items;
        DebugExtensions.DebugExtension.Log(items);
        scrollDownStart = slots / 2 + 1;
        scrollUpStart = slots / 2 - 1;
        RefreshViewFrame();
    }
}
