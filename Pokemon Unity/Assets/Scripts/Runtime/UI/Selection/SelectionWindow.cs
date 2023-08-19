using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class SelectionWindow : ClosableView
{
    public static readonly int PreviousSelection = -1;
    public static readonly int NoSelectionOnStart = -2;

    [SerializeField] protected SelectableUIElement[] elements;
    [SerializeField] private AudioClip selectSound;

    protected SelectableUIElement[] _elements;

    public SelectableUIElement selectedElement => _elements is null ? null : _elements[selectedIndex];

    public int selectedIndex { get; protected set; } = 0;

    private bool forceSelection = false;
    private int previousSelection = -1;

    public override void Open() => Open(null, false, PreviousSelection);
    public override void Open(Action<SelectableUIElement, bool> callback) => Open(callback, false, PreviousSelection);
    public virtual void Open(Action<SelectableUIElement, bool> callback, bool forceSelection) => Open(callback, forceSelection, PreviousSelection);
    public virtual void Open(Action<SelectableUIElement, bool> callback, int startSelection) => Open(callback, false, startSelection);
    public virtual void Open(Action<SelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        startSelection = startSelection == NoSelectionOnStart ? NoSelectionOnStart :
            startSelection == PreviousSelection ? previousSelection :
            startSelection;

        if (_elements is null)
            AssignElements();

        bool dontSelectAnything = startSelection == NoSelectionOnStart;

        if (!dontSelectAnything)
            startSelection = Math.Clamp(startSelection, 0, elements.Length - 1);

        print($"Open {gameObject.name} {startSelection}");
        this.forceSelection = forceSelection;

        for (int i = 0; i < _elements.Length; i++)
        {
            _elements[i].Initialize(i);
            if (i != startSelection)
                _elements[i].Deselect();
        }

        print(startSelection != NoSelectionOnStart);
        if (!dontSelectAnything)
            SelectElement(startSelection, false);
        
        base.Open(callback);
    }

    public override void Close()
    {
        print($"Close {gameObject.name}, set previous to {selectedIndex}");
        previousSelection = selectedIndex;
        if (selectedElement != null) selectedElement.Deselect();
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

    public bool ProcessInputChancel(InputData input) => base.ProcessInput(input);

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

    virtual protected void SelectElement(int index, bool playSound) => SelectElement(index, true, playSound);
    virtual protected void SelectElement(int index, bool deselectPreviouslySelected, bool playSound)
    {
        if (deselectPreviouslySelected)
            selectedElement.Deselect();
        selectedIndex = index;
        selectedElement.Select();
        if (playSound) SfxHandler.Play(selectSound);
    }

    virtual protected void SelectElement(SelectableUIElement element) => SelectElement(element is null ? selectedIndex : element.GetIndex(), true);
    virtual protected void ChooseSelectedElement() => callback?.Invoke(selectedElement, false);/* TODO sound*/

    public void DeselectSelection() => selectedElement.Deselect();

    protected override void GoBack()
    {
        if (!forceSelection)
            base.GoBack();
    }
}
