using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OpenedInputConsumer : UIView, IInputConsumer
{
    public override void Open()
    {
        base.Open();
        InputManager.Instance.Register(this);
    }

    public override void Close()
    {
        base.Close();
        InputManager.Instance.Unregister(this);
    }

    public virtual bool ProcessInput(InputData input) => false;
}
