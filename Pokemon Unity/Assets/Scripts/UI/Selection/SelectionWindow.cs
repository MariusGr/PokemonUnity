using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class SelectionWindow : ClosableView, ISelectionWindow
{
    [SerializeField] public SelectableUIElement[] elements;

    private bool forceSelection = false;
    protected int selectedIndex = 0;
    protected SelectableUIElement selectedElement => elements[selectedIndex];

    public override void Open() => Open(null, false, 0);
    public override void Open(Action<ISelectableUIElement, bool> callback) => Open(callback, false, 0);
    public virtual void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection) => Open(callback, forceSelection, 0);
    public virtual void Open(Action<ISelectableUIElement, bool> callback, int startSelection) => Open(callback, false, startSelection);
    public virtual void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        this.forceSelection = forceSelection;

        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].Initialize(i);
            if (i != startSelection)
                elements[i].Deselect();
        }

        SelectElement(startSelection);
        base.Open(callback);
    }

    public override void Close()
    {
        selectedElement.Deselect();
        base.Close();
    }

    virtual public void RefreshElement(int index)
    {
        if (index > -1) elements[index].Refresh();
    }

    public override bool ProcessInput(InputData input)
    {
        if (!base.ProcessInput(input))
            if (input.submit.pressed)
            {
                ChooseSelectedElement();
                return true;
            }
        return false;
    }

    public bool ProcessInputChancel(InputData input)
    {
        return base.ProcessInput(input);
    }

    public virtual void AssignElements(object[] elements)
    {
        for (int i = 0; i < elements.Length; i++)
            this.elements[i].AssignElement(elements[i]);

        for (int i = elements.Length; i < this.elements.Length; i++)
            this.elements[i].AssignNone();
    }

    public void AssignOnSelectCallback(Action<object> callback)
    {
        foreach (SelectableUIElement element in elements)
            element.AssignOnSelectCallback(callback);
    }

    virtual protected void SelectElement(int index)
    {
        selectedElement.Deselect();
        selectedIndex = index;
        selectedElement.Select();
    }

    virtual protected void SelectElement(SelectableUIElement element) => SelectElement(element is null ? selectedIndex : element.index);
    virtual protected void ChooseSelectedElement() => callback?.Invoke(selectedElement, false);/* TODO sound*/

    protected override void GoBack()
    {
        if (!forceSelection)
            base.GoBack();
    }
}
