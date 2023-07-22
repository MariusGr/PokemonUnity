using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DialogEvent : StoryEventData
{
    public string text;
    public bool tryInteract;

    public override IEnumerator Invoke()
    {
        if (tryInteract)
            PlayerCharacter.Instance.TryInteract();

        yield return GlobalDialogBox.Instance.DrawText(text, DialogBoxContinueMode.User, true);
    }
}
