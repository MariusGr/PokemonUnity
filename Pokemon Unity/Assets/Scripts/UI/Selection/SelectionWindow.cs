using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class SelectionWindow : OpenedInputConsumer, ISelectionWindow
{
    [SerializeField] public SelectableUIElement[] elements;

    private int selectedIndex = 0;
    protected SelectableUIElement selectedElement => elements[selectedIndex];
    bool forceSelection = false;
    Action<ISelectableUIElement, bool> callback;

    public override void Open() => Open(null, false, 0);
    public virtual void Open(Action<ISelectableUIElement, bool> callback) => Open(callback, false, 0);
    public virtual void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection) => Open(callback, forceSelection, 0);
    public virtual void Open(Action<ISelectableUIElement, bool> callback, int startSelection) => Open(callback, false, startSelection);
    public virtual void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        this.callback = callback;
        this.forceSelection = forceSelection;
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].Initialize(i);
            if (i != startSelection)
                elements[i].Deselect();
        }

        SelectElement(startSelection);
        print(selectedElement.gameObject.name);
        base.Open();
        print(this);
        print(gameObject.name);
    }

    public override void Close() => base.Close();

    virtual public void RefreshElement(int index)
    {
        if (index > -1) elements[index].Refresh();
    }

    public override bool ProcessInput(InputData input)
    {
        if (input.submit.pressed)
        {
            ChooseSelectedElement();
            return true;
        }
        if (!forceSelection && input.chancel.pressed)
        {
            GoBack();
            return true;
        }
        return false;
    }

    public void AssignElements(object[] elements)
    {
        for (int i = 0; i < elements.Length; i++)
        {
            this.elements[i].AssignElement(elements[i]);
        }

        for (int i = elements.Length; i < this.elements.Length; i++)
            this.elements[i].AssignNone();
    }

    virtual protected void SelectElement(int index)
    {
        selectedElement.Deselect();
        selectedIndex = index;
        selectedElement.Select();
    }

    virtual protected void SelectElement(SelectableUIElement element) => SelectElement(element is null ? selectedIndex : element.index);
    private void ChooseSelectedElement() => callback?.Invoke(selectedElement, false);/* TODO sound*/ 
    virtual protected void GoBack() => callback?.Invoke(null, true);
}
