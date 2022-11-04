using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectionWindow : OpenedInputConsumer
{
    [SerializeField] public SelectableUIElement[] elements;

    private int selectedIndex = 0;
    protected SelectableUIElement selectedElement => elements[selectedIndex];
    bool forceSelection = false;

    virtual public void Initialize(int startSelection = 0)
    {
        for (int i = 0; i < elements.Length; i++)
            elements[i].Initialize(i);
        SelectElement(startSelection);
    }

    public void Open(bool forceSelection)
    {
        this.forceSelection = forceSelection;
        Open();
    }

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
            this.elements[i].AssignElement(elements[i]);
        for (int i = elements.Length; i < this.elements.Length; i++)
            this.elements[i].AssignNone();
    }

    virtual protected void ChooseSelectedElement() { /* TODO sound*/ }
    virtual protected void SelectElement(int index)
    {
        selectedElement.Deselect();
        selectedIndex = index;
        selectedElement.Select();
    }

    virtual protected void SelectElement(SelectableUIElement element) => SelectElement(element is null ? selectedIndex : element.index);
    virtual protected void GoBack() { }
}
