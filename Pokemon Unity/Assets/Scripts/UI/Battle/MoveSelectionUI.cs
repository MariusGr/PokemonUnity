using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionUI : SelectionWindow
{
    private void Awake() => Initialize();
    private void Update() => ProcessInput();

    protected override void ChooseSelectedElement()
    {
        base.ChooseSelectedElement();
        print("Select Pkmn");
        Services.Get<IBattleManager>().ChoosePlayerMove(((MoveButton)selectedElement).move, false);
    }

    protected override void GoBack()
    {
        base.GoBack();
        Services.Get<IBattleManager>().ChoosePlayerMove(null, false);
    }
}
