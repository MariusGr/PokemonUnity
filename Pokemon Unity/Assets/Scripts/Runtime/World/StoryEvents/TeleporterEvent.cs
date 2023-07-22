using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TeleporterEvent : StoryEventData
{
    public string targetKey;
    public Direction Direction;

    public override IEnumerator Invoke()
    {
        if (!TeleportTarget.TryGet(targetKey, out TeleportTarget target))
        {
            Debug.LogError($"TeleportTarget with {targetKey} not found!");
            yield break;
        }

        EventManager.Pause();
        yield return FadeBlack.Instance.FadeToBlack();
        target.TeleportTo(Direction);
        yield return FadeBlack.Instance.FadeFromBlack();
        EventManager.Unpause();
    }
}
