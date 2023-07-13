using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StoryEvent : BaseScriptableObject
{
    private bool happened = false;

    public void TryInvoke()
    {
        Debug.Log(happened);
        if (!happened)
            Invoke();
    }

    virtual protected void Invoke()
    {
        happened = true;
    }
}
