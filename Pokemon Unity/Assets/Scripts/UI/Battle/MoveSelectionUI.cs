using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionUI : SelectionWindow
{
    [SerializeField] public MoveButton[] buttons;

    private void Awake()
    {
        selectedElement = buttons[0];
    }

    private void Update()
    {
        ProcessInput();
    }

    public void AssignMoves(Move[] moves)
    {
        for (int i = 0; i < moves.Length; i++)
            buttons[i].AssignMove(moves[i]);
        for (int i = moves.Length; i < 4; i++)
            buttons[i].AssignNone();
    }

    protected override void SelectElement()
    {
        Services.Get<IBattleManager>().ChoosePlayerMove(((MoveButton)selectedElement).move);
    }
}
