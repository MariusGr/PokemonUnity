using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICallbackUIView : IUIView
{
    public void Open(System.Action<ISelectableUIElement, bool> callback);
}
