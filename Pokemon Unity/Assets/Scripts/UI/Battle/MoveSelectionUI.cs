using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionUI : SelectionWindow
{
    private void Awake()
    {
        selectedElement = buttons[0];
    }

    private void Update()
    {
        ProcessInput();
    }

    protected override void SelectElement()
    {
        print("Select");
        Services.Get<IBattleManager>().ChoosePlayerMove(((MoveButton)selectedElement).move);
    }
}
