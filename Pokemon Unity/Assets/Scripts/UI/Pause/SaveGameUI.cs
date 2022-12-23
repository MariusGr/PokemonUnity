using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveGameUI : UIView
{
    [SerializeField] private AudioClip saveGameSound;

    private Action goBackCallback;

    public void Open(Action goBackCallback)
    {
        base.Open();
        this.goBackCallback = goBackCallback;
        StartCoroutine(AskForSaving());
    }

    public override void Close()
    {
        GlobalDialogBox.Instance.Close();
        base.Close();
    }

    private IEnumerator AskForSaving()
    {
        yield return GlobalDialogBox.Instance.DrawChoiceBox("Möchtest du das Spiel speichern?");
        if (GlobalDialogBox.Instance.chosenIndex == 0)
        {
            GlobalDialogBox.Instance.DrawText("Spiel wird gespeichert...", DialogBoxContinueMode.External);
            SaveGameManager.Instance.SaveGame();
            SfxHandler.Play(saveGameSound);
            yield return GlobalDialogBox.Instance.DrawText("Spiel wurde gespeichert!", DialogBoxContinueMode.User);
        }

        goBackCallback?.Invoke();
    }
}
