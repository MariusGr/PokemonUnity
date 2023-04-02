using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClosableView : InputConsumer
{
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip closeSound;

    protected Action<ISelectableUIElement, bool> callback;

    public override void Open() => Open(null);
    public virtual void Open(Action<ISelectableUIElement, bool> callback)
    {
        this.callback = callback;
        base.Open();
        SfxHandler.Play(openSound);
    }

    public override void Close()
    {
        base.Close();
        SfxHandler.Play(closeSound);
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
