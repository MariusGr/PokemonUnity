using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StoryEvent : BaseScriptableObject
{
    private bool disabled = false;

    public void TryInvoke()
    {
        if (!disabled)
            Invoke();
    }

    virtual protected void Invoke()
    {
        disabled = true;
    }
}
