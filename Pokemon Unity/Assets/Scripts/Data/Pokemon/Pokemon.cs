using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pokemon
{
    //https://bulbapedia.bulbagarden.net/wiki/Stat#Stage_multipliers
    private static readonly float[] statStageMultipliers = new float[]
    {
        2f / 8f,
        2f / 7f,
        2f / 6f,
        2f / 5f,
        2f / 4f,
        2f / 3f,
        1f,
        3f / 2f,
        4f / 2f,
        5f / 2f,
        6f / 2f,
        7f / 2f,
        8f / 2f,
    };
    private static float[] accuracyStageMultipliers = new float[]
    {
        33f / 100f,
        36f / 100f,
        43f / 100f,
        50f / 100f,
        60f / 100f,
        75f / 100f,
        1f,
        133f / 100f,
        166f / 100f,
        200f / 100f,
        250f / 100f,
        266f / 100f,
        300f / 100f,
    };

    public class StageMultiplier
    {
        private float[] multipliers;
        private int _stage;
        public int stage
        {
            get => _stage - 6;
            set => _stage = Math.Max(0, Math.Min(multipliers.Length, value + 6));
        }
        public float multiplier => multipliers[_stage];

        public StageMultiplier(float[] multipliers)
        {
            _stage = 6;
            this.multipliers = multipliers;
        }

        public float GetMultiplier(int substractStage = 0) => multipliers[Math.Max(0, Math.Min(multipliers.Length, stage - substractStage + 6))];
    }

    public PokemonData data;
    public string nickname = null;

    public int level;
    // TODO increase stats with level
    public int attack => (int)(BasteStatToStat(data.attack) * stageAttack.multiplier);
    public int specialAttack => (int)(BasteStatToStat(data.specialAttack) * stageSpecialAttack.multiplier);
    public int defense => (int)(BasteStatToStat(data.defense) * stageDefense.multiplier);
    public int specialDefense => (int)(BasteStatToStat(data.specialDefense) * stageSpecialDefense.multiplier);
    public int speed => (int)(BasteStatToStat(data.speed) * stageSpeed.multiplier);
    public int maxHp => BasteStatToStat(data.maxHp);

    public List<Move> moves;
    public int hp;
    public int xp;
    public int nextLevelXp = 1;
    public Status status = Status.None;
    public bool isFainted => hp < 1;
    public StageMultiplier stageAttack;
    public StageMultiplier stageSpecialAttack;
    public StageMultiplier stageDefense;
    public StageMultiplier stageSpecialDefense;
    public StageMultiplier stageSpeed;
    public StageMultiplier stageAccuracy;
    public StageMultiplier stageEvasion;

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
        stageAttack = new StageMultiplier(statStageMultipliers);
        stageSpecialAttack = new StageMultiplier(statStageMultipliers);
        stageDefense = new StageMultiplier(statStageMultipliers);
        stageSpecialDefense = new StageMultiplier(statStageMultipliers);
        stageSpeed = new StageMultiplier(statStageMultipliers);
        stageAccuracy = new StageMultiplier(accuracyStageMultipliers);
        stageEvasion = new StageMultiplier(accuracyStageMultipliers);

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

    public bool HasUsableMoves()
    {
        foreach (Move move in moves)
            if (move.isUsable)
                return true;
        return false;
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

        foreach (Move move in moves)
            move.ReplenishPP();
        // TODO: HEal status effects
    }
}
