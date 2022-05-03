using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovesSelectionUI : SelectionWindow
{
    protected override void SelectElement()
    {
        Services.Get<IBattleManager>().DoMove(((MoveButton)selectedElement).move);
    }
}
