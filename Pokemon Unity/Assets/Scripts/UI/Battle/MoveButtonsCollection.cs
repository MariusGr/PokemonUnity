using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButtonsCollection : MonoBehaviour
{
    [SerializeField] public MoveButton[] buttons;

    public void AssignMoves(Move[] moves)
    {
        for (int i = 0; i < moves.Length; i++)
            buttons[i].AssignMove(moves[i]);
        for (int i = moves.Length; i < 4; i++)
            buttons[i].AssignNone();
    }
}
