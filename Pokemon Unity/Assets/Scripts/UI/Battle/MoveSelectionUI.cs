using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionUI : SelectionGraphWindow
{
    private void Awake() => Initialize();

    protected override void ChooseSelectedElement()
    {
        base.ChooseSelectedElement();
        print("Choose Move");
        Services.Get<IBattleManager>().ChoosePlayerMove(((MoveButton)selectedElement).move, false);
    }

    protected override void GoBack()
    {
        base.GoBack();
        print("back");
        Services.Get<IBattleManager>().ChoosePlayerMove(null, true);
    }
}
