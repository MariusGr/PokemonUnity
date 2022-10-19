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
    // TODO increase stats with level
    public int attack => BasteStatToStat(data.attack);
    public int defense => BasteStatToStat(data.defense);
    public int speed => BasteStatToStat(data.speed);
    public int maxHp => BasteStatToStat(data.maxHp);

    public List<Move> moves;
    public int hp;
    public int xp;
    public int nextLevelXp = 1;
    public Status status = Status.None;
    public bool isFainted => hp < 1;

    public float hpNormalized => (float)hp / maxHp;
    public float xpNormalized => xp / nextLevelXp;

    public string Name => nickname is null || nickname.Length < 1 ? data.fullName : nickname;
    public Gender gender;

    int BasteStatToStat(int baseStat) => baseStat + baseStat * level / 50;

    public Pokemon(EncounterPokemon encounterPokemonData) :
        this(encounterPokemonData.data, UnityEngine.Random.Range(encounterPokemonData.minLevel, encounterPokemonData.maxLevel))
    { }

    public Pokemon(PokemonData pokemonData, int level)
    {
        this.data = pokemonData;
        this.level = level;
        Initialize();
    }

    public void Initialize()
    {
        hp = maxHp;
        moves = new List<Move>();
        foreach (int key in data.levelToMoveDataMap.keys)
            if (key <= level)
                AddMove(data.levelToMoveDataMap[key]);

        if (gender is null) gender = Gender.GetRandomGender();

        nextLevelXp = 1;
    }

    private void AddMove(MoveData moveData)
    {
        Move move = new Move(moveData, moves.Count, this);
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
        Debug.Log($"Inflict {damage} on currently {hp}");
        hp -= damage;
        if (hp < 1)
        {
            hp = 0;
            return true;
        }
        return false;
    }

    public void Heal()
    {
        hp = maxHp;
        // TODO: HEal status effects
    }
}
