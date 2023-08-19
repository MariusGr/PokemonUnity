using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ScrollSelection : ScalarSelection
{
    [SerializeField] protected int slots = 6;
    [SerializeField] protected ShadowedText description;

    private object[] items;
    private int viewFrameStart = 0;
    private int shownItemsEnd => viewFrameStart + slots;
    private int scrollDownStart;
    private int scrollUpStart;

    public override void Open(Action<SelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        print("Open " + GetType());
        viewFrameStart = 0;
        base.Open(callback, forceSelection, startSelection);
    }

    protected override void SelectElement(int index, bool playSound)
    {
        print("Select " + gameObject.name);
        if (items.Length > slots)
        {
            // scrolling is necessary since not all items fit into the view
            if (index > scrollDownStart && shownItemsEnd < items.Length)
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

        base.SelectElement(index, playSound);
    }

    protected virtual void MoveViewFrame(int shift)
    {
        viewFrameStart = viewFrameStart + shift;
        RefreshViewFrame();
    }
    protected void RefreshViewFrame() => AssignElements(items.Skip(viewFrameStart).Take(slots).ToArray());

    public void AssignItems(object[] items)
    {
        if (!(this.items is null) && this.items.Length > items.Length)
            viewFrameStart = Math.Max(0, viewFrameStart - (this.items.Length - items.Length));

        this.items = items;
        scrollDownStart = slots / 2 + 1;
        scrollUpStart = slots / 2 - 1;
        RefreshViewFrame();
    }
}
