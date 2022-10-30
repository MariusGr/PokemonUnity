using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectionWindow : MonoBehaviour
{
    [SerializeField] public SelectableUIElement[] elements;

    private int selectedIndex = 0;
    protected SelectableUIElement selectedElement => elements[selectedIndex];

    virtual public void Initialize()
    {
        for (int i = 0; i < elements.Length; i++)
            elements[i].index = i;
        SelectElement(0);
    }

    virtual protected bool ProcessInput()
    {
        if (Input.GetButtonDown("Submit"))
        {
            ChooseSelectedElement();
            return true;
        }
        if (Input.GetButtonDown("Back"))
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
        for (int i = elements.Length; i < 4; i++)
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
