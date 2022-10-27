using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectionWindow : MonoBehaviour
{
    [SerializeField] public SelectableUIElement[] buttons;

    protected SelectableUIElement selectedElement;

    virtual public void Initialize(SelectableUIElement startElements)
    {
        selectedElement = startElements;
    }

    virtual protected void ProcessInput()
    {
        if (Input.GetButtonDown("Submit"))
            SelectElement();
        //else if(Input.GetButtonDown("Submit"))
    }

    public void AssignElements(object[] elements)
    {
        for (int i = 0; i < elements.Length; i++)
            buttons[i].AssignElement(elements[i]);
        for (int i = elements.Length; i < 4; i++)
            buttons[i].AssignNone();
    }

    abstract protected void SelectElement();
}
