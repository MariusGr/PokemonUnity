using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Move
{
    public MoveData data;
    public int index;
    public int pp;

    private Pokemon pokemon;

    public Move(Pokemon pokemon, MoveData data, int index)
    {
        this.pokemon = pokemon;
        this.index = index;
        this.data = data;
        pp = data.maxPP;
    }
}
