using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectionWindow : MonoBehaviour
{
    protected SelectableUIElement selectedElement;

    virtual public void Initialize(SelectableUIElement startElements)
    {
        selectedElement = startElements;
    }

    virtual protected void ProcesInput()
    {
        if (Input.GetButtonDown("Submit"))
            SelectElement();
        //else if(Input.GetButtonDown("Submit"))
    }

    abstract protected void SelectElement();
}
