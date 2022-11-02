using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectionWindow : MonoBehaviour, IInputConsumer
{
    [SerializeField] public SelectableUIElement[] elements;

    private int selectedIndex = 0;
    protected SelectableUIElement selectedElement => elements[selectedIndex];
    bool pauseInputProcessing = false;

    virtual public void Initialize()
    {
        for (int i = 0; i < elements.Length; i++)
            elements[i].Initialize(i);
        SelectElement(0);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        InputManager.Instance.Register(this);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        InputManager.Instance.Unregister(this);
    }

    virtual public void RefreshElement(int index)
    {
        if (index > -1) elements[index].Refresh();
    }

    public virtual bool ProcessInput(InputData input)
    {
        if (pauseInputProcessing)
            return false;

        if (input.submit.pressed)
        {
            ChooseSelectedElement();
            return true;
        }
        if (input.chancel.pressed)
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
