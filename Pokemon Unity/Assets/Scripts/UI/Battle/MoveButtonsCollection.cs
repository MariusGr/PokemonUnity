using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButtonsCollection : MonoBehaviour
{
    [SerializeField] private MoveButton[] buttons;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AssignMoves(Move[] moves)
    {
        for (int i = 0; i < moves.Length; i++)
            buttons[i].AssignMove(moves[i]);
    }
}
