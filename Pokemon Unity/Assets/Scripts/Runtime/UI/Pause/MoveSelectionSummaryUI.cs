using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionSummaryUI : MoveSelectionUI
{
    public void Show() => gameObject.SetActive(true);

    public override void Close()
    {
        base.Close();
        Show();
    }
}
