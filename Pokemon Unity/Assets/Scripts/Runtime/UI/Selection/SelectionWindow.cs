using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class SelectionWindow : ClosableView
{
    [SerializeField] protected SelectableUIElement[] elements;
    [SerializeField] private AudioClip selectSound;

    protected SelectableUIElement[] _elements;

    public SelectableUIElement selectedElement => _elements is null ? null : _elements[selectedIndex];

    public int selectedIndex { get; protected set; } = 0;

    private bool forceSelection = false;

    public override void Open() => Open(null, false, 0);
    public override void Open(Action<SelectableUIElement, bool> callback) => Open(callback, false, 0);
    public virtual void Open(Action<SelectableUIElement, bool> callback, bool forceSelection) => Open(callback, forceSelection, 0);
    public virtual void Open(Action<SelectableUIElement, bool> callback, int startSelection) => Open(callback, false, startSelection);
    public virtual void Open(Action<SelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        this.forceSelection = forceSelection;

        if (_elements is null)
            AssignElements();

        for (int i = 0; i < _elements.Length; i++)
        {
            _elements[i].Initialize(i);
            if (i != startSelection)
                _elements[i].Deselect();
        }

        AudioClip selectSoundBackup = selectSound;
        selectSound = null;
        if (startSelection > _elements.Length - 1)
            SelectElement(_elements.Length - 1);
        else if (startSelection > -1)
            SelectElement(startSelection);
        selectSound = selectSoundBackup;
        base.Open(callback);
    }

    public override void Close()
    {
        selectedElement?.Deselect();
        base.Close();
    }

    virtual public void RefreshElement(int index)
    {
        if (index > -1) _elements[index].Refresh();
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

    public virtual void AssignElements() => AssignElements(new object[elements.Length]);
    public virtual void AssignElements(object[] payloads)
    {
        _elements = new SelectableUIElement[elements.Length];

        for (int i = 0; i < payloads.Length; i++)
        {
            _elements[i] = elements[i];
            _elements[i].AssignElement(payloads[i]);
        }

        for (int i = payloads.Length; i < _elements.Length; i++)
        {
            _elements[i] = elements[i];
            _elements[i].AssignNone();
        }
    }

    public void AssignOnSelectCallback(Action<object> callback)
    {
        foreach (SelectableUIElement element in _elements)
            element.AssignOnSelectCallback(callback);
    }

    virtual protected void SelectElement(int index) => SelectElement(index, true);
    virtual protected void SelectElement(int index, bool deselectPreviouslySelected)
    {
        if (deselectPreviouslySelected)
            selectedElement.Deselect();
        selectedIndex = index;
        selectedElement.Select();
        SfxHandler.Play(selectSound);
    }

    virtual protected void SelectElement(SelectableUIElement element) => SelectElement(element is null ? selectedIndex : element.GetIndex());
    virtual protected void ChooseSelectedElement() => callback?.Invoke(selectedElement, false);/* TODO sound*/
    public void DeselectSelection() => selectedElement.Deselect();

    protected override void GoBack()
    {
        if (!forceSelection)
            base.GoBack();
    }
}