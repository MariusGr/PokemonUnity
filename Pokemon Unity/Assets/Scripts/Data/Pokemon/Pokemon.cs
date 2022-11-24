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
    public CharacterData character { get; private set; }
    public int id;
    public string nickname = null;

    public DateTime metDate;
    public string metMap = "Entonhausen";
    public string metLevel;

    public int level;
    // TODO increase stats with level
    public int attack => (int)(BasteStatToStat(data.attack) * stageAttack.multiplier);
    public int specialAttack => (int)(BasteStatToStat(data.specialAttack) * stageSpecialAttack.multiplier);
    public int defense => (int)(BasteStatToStat(data.defense) * stageDefense.multiplier);
    public int specialDefense => (int)(BasteStatToStat(data.specialDefense) * stageSpecialDefense.multiplier);
    public int speed => (int)(speedUnmodified * stageSpeed.multiplier);
    public int speedUnmodified => BasteStatToStat(data.speed);
    public int maxHp => BasteStatToStat(data.maxHp);

    public List<Move> moves;
    public int hp;
    public int xp;
    public int xpNeededForNextLevel => data.GetXPForLevel(level + 1);
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
    public float xpNormalized
    {
        get
        {
            int xpMilestoneCurrent = data.GetXPForLevel(level);
            Debug.Log($"current {xpMilestoneCurrent} next {xpNeededForNextLevel}, xp {xp} norm {(xp - xpMilestoneCurrent) / (xpNeededForNextLevel - xpMilestoneCurrent)}");

            return Math.Min(1f, (xp - xpMilestoneCurrent) / (float)(xpNeededForNextLevel - xpMilestoneCurrent));
        }
    }

    public string Name => nickname is null || nickname.Length < 1 ? data.fullName.ToUpper() : nickname;
    public Gender gender;

    private int BasteStatToStat(int baseStat) => baseStat + baseStat * level / 50;

    public Pokemon(EncounterPokemon encounterPokemonData) :
        this(encounterPokemonData.data, UnityEngine.Random.Range(encounterPokemonData.minLevel, encounterPokemonData.maxLevel), null)
    { }

    public Pokemon(PokemonData pokemonData, int level, CharacterData character)
    {
        this.data = pokemonData;
        this.level = level;
        Initialize(character);
    }

    public void Initialize(CharacterData character)
    {
        this.character = character;
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
        xp = data.GetXPForLevel(level);
    }

    public void AddMove(MoveData moveData) => moves.Add(new Move(moveData, moves.Count, this));
    public void ReplaceMove(Move move, MoveData newMove) => moves[move.index] = new Move(newMove, move.index, this);

    public void SwapMoves(Move move1, Move move2)
    {
        if (move1 == move2)
            return;
        int newIndex1 = move2.index;
        int newIndex2 = move1.index;
        moves[newIndex1] = move1;
        moves[newIndex2] = move2;
        move1.index = newIndex1;
        move2.index = newIndex2;
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

    // https://bulbapedia.bulbagarden.net/wiki/Experience
    public int GetXPGainedFromFaint(bool opponentIsWild)
    {
        float a = opponentIsWild ? 1f : 1.5f;
        return (int)((data.baseXPGain * level / 7f) * a);
    }

    public int GainXP(int xp) => IsLeveledToMax() ? this.xp = data.GetXPForLevel(100) : this.xp += xp;
    public void GrowLevel()
    {
        // TODO: learn new moves
        float hpBefore = hpNormalized;
        level++;
        hp = (int)Mathf.Ceil(hpBefore * maxHp);
    }

    public bool WillGrowLevel() => xp >= xpNeededForNextLevel && !IsLeveledToMax();
    public bool IsLeveledToMax() => level >= 100;
    public bool WillEvolve() => data.evolutionLevel > 1 && !(data.evolution is null) && level >= data.evolutionLevel;

    public Pokemon GetEvolvedVersion()
    {
        Pokemon evolved = new Pokemon(data.evolution, level, character);
        evolved.xp = xp;
        evolved.hp = (int)(evolved.maxHp * hpNormalized);
        evolved.gender = gender;
        evolved.nickname = nickname;
        evolved.moves = new List<Move>();
        foreach (Move m in moves)
            evolved.moves.Add(m);
        return evolved;
    }
}
