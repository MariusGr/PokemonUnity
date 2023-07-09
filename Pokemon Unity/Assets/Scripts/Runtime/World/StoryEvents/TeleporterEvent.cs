using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeleporterEvent", menuName = "Story Events/Teleporter Event")]
public class TeleporterEvent : StoryEvent
{
    public Transform target;

    protected override void Invoke()
    {
        base.Invoke();
        
    }
}
