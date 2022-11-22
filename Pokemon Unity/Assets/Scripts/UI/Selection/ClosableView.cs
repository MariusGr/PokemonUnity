using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClosableView : InputConsumer
{
    protected Action<ISelectableUIElement, bool> callback;

    public override void Open() => Open(null);
    public virtual void Open(Action<ISelectableUIElement, bool> callback)
    {
        this.callback = callback;
        base.Open();
    }

    public override bool ProcessInput(InputData input)
    {
        if (input.chancel.pressed)
        {
            GoBack();
            return true;
        }
        return false;
    }

    virtual protected void GoBack() => callback?.Invoke(null, true);
}
