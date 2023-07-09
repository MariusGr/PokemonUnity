using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogEvent", menuName = "Story Events/Dialog Event")]
public class DialogEvent : StoryEvent
{
    public string text;

    protected override void Invoke()
    {
        base.Invoke();
    }
}
