using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScalarSelection : SelectionWindow
{
    [SerializeField] private Direction positiveDirection = Direction.Right;
    [SerializeField] private Direction negativeDirection = Direction.Left;
    [SerializeField] private bool looping = true;

    private Func<InputData, bool> otherAxisInputCallback;

    public void Open(Action<SelectableUIElement, bool> callback, int startSelection, Func<InputData, bool> otherAxisInputCallback)
        => Open(callback, false, startSelection, otherAxisInputCallback);

    public void Open(Action<SelectableUIElement, bool> callback, bool forceSelection, int startSelection, Func<InputData, bool> otherAxisInputCallback)
    {
        this.otherAxisInputCallback = otherAxisInputCallback;
        base.Open(callback, forceSelection, startSelection);
    }

    public override bool ProcessInput(InputData input)
    {
        if (!base.ProcessInput(input))
        {
            if (input.digitalPad.pressed == positiveDirection)
                return TrySelectPositive();
            if (input.digitalPad.pressed == negativeDirection)
                return TrySelectNegative();
            if (input.digitalPad.pressed != Direction.None)
            {
                otherAxisInputCallback?.Invoke(input);
                return false;
            }
        }
        return false;
    }

    protected virtual bool TrySelectPositive()
    {
        int nextIndex = -1;

        if (looping)
            nextIndex = (selectedIndex + 1) % _elements.Length;
        else if (selectedIndex < _elements.Length - 1)
            nextIndex = selectedIndex + 1;
        else
            return false;
        if (_elements[nextIndex].IsAssigned())
        {
            SelectElement(nextIndex, true);
            return true;
        }
        return false;
    }

    protected virtual bool TrySelectNegative()
    {
        int nextIndex = -1;

        if (looping)
            nextIndex = selectedIndex < 1 ? _elements.Length - 1 : selectedIndex - 1;
        else if (selectedIndex > 0)
            nextIndex = selectedIndex - 1;
        else
            return false;
        if (_elements[nextIndex].IsAssigned())
        {
            SelectElement(nextIndex, true);
            return true;
        }
        return false;
    }
}
