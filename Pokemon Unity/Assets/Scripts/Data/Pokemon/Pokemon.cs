using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Pokemon
{
    public PokemonData data;
    public float level;
    public List<Move> moves = new List<Move>();

    public void OnAfterDeserialize()
    {
        foreach(int key in data.levelToMoveMap.keys)
        {
            if (key <= level)
                moves.Add(xz);
        }
    }
    
    private void AddMove(MoveData moveData)
    {
        Move move = new Move(this, moveData, moves.Length);
        moves.Add()
    }
}
