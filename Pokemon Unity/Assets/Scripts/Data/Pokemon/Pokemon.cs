using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pokemon
{
    public PokemonData data;
    public string nickname = null;

    public int level;
    public int attack;
    public int defense;

    public List<Move> moves;
    public int hp;
    public int xp;
    public int nextLevelXp = 1;
    public Status status = Status.None;

    public float hpNormalized => hp / data.maxHp;
    public float xpNormalized => xp / nextLevelXp;

    public string Name => nickname.Length < 1 || nickname is null ? data.name : nickname;
    public Gender gender;

    public void Initialize()
    {
        hp = data.maxHp;
        moves = new List<Move>();
        foreach (int key in data.levelToMoveDataMap.keys)
            if (key <= level)
                AddMove(data.levelToMoveDataMap[key]);

        if (gender is null) gender = Gender.GetRandomGender();

        nextLevelXp = 1;
    }

    private void AddMove(MoveData moveData)
    {
        Move move = new Move(moveData, moves.Count);
        moves.Add(move);
    }

    public bool MatchesType(PokemonTypeData type)
    {
        foreach (PokemonTypeData myType in data.pokemonTypes)
            if (myType == type)
                return true;
        return false;
    }

    // return true when dead
    public bool InflictDamage(int damage)
    {
        hp -= damage;
        if (hp < 1)
        {
            hp = 0;
            return true;
        }
        return false;
    }
}
