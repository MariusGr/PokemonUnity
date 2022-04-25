using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pokemon
{
    public PokemonData data;
    public float level;
    public List<Move> moves;
    public int hp;

    public void Initialize()
    {
        hp = data.maxHp;
        moves = new List<Move>();
        foreach (int key in data.levelToMoveDataMap.keys)
        {
            if (key <= level)
                AddMove(data.levelToMoveDataMap[key]);
        }
    }

    private void AddMove(MoveData moveData)
    {
        Move move = new Move(this, moveData, moves.Count);
        moves.Add(move);
    }
}
