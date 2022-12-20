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
        protected int index;
        public int stage
        {
            get => IndexToStage(index);
            set => index = StageToIndex(value);
        }
        protected float multiplier => multipliers[index];

        public StageMultiplier(float[] multipliers)
        {
            index = 6;
            this.multipliers = multipliers;
        }

        public float GetMultiplier() => multiplier;
        public virtual float GetMultiplier(bool criticalHit) => multiplier;
        private int StageToIndex(int stage) => Math.Max(0, Math.Min(multipliers.Length, stage + 6));
        private int IndexToStage(int index) => index - 6;
        protected float GetMultiplier(int stage) => multipliers[StageToIndex(stage)];
        //public float GetMultiplier(int substractStage = 0) => multipliers[Math.Max(0, Math.Min(multipliers.Length, stage + substractStage + 6))];
    }

    public class StageMultiplierOffensive : StageMultiplier
    {
        public StageMultiplierOffensive(float[] multipliers) : base(multipliers) { }
        public override float GetMultiplier(bool criticalHit) => criticalHit ? GetMultiplier(Math.Max(0, stage)) : multiplier;
    }

    public class StageMultiplierDefensive : StageMultiplier
    {
        public StageMultiplierDefensive(float[] multipliers) : base(multipliers) { }
        public override float GetMultiplier(bool criticalHit) => criticalHit ? GetMultiplier(Math.Min(0, stage)) : multiplier;
    }

    public PokemonData data;
    public CharacterData character { get; private set; }
    public int id;
    public string nickname = null;

    public DateTime metDate;
    public string metMap = "Entonhausen";
    public string metLevel;

    public int level;
    // TODO show stats without status effect multipliers for displaying puposes
    public int GetAttack(bool criticalHit) => (int)(attackUnmodified * stageAttack.GetMultiplier(criticalHit));
    public int GetSpecialAttack(bool criticalHit) => (int)(specialAttackUnmodified * stageSpecialAttack.GetMultiplier(criticalHit));
    public int GetDefense(bool criticalHit) => (int)(defenseUnmodified * stageDefense.GetMultiplier(criticalHit));
    public int GetSpecialDefense(bool criticalHit) => (int)(specialDefenseUnmodified * stageSpecialDefense.GetMultiplier(criticalHit));
    public int speed => (int)(speedUnmodified * stageSpeed.GetMultiplier() * (statusEffect is null ? 1f : statusEffect.statModifierSpeedRelative));

    public int attackUnmodified => BasteStatToStat(data.attack);
    public int specialAttackUnmodified => BasteStatToStat(data.specialAttack);
    public int defenseUnmodified => BasteStatToStat(data.defense);
    public int specialDefenseUnmodified => BasteStatToStat(data.specialDefense);
    public int speedUnmodified => BasteStatToStat(data.speed);
    public int maxHp => BasteStatToStat(data.maxHp);

    public List<Move> moves;
    public int hp;
    public int xp;
    public int xpNeededForNextLevel => data.GetXPForLevel(level + 1);
    [SerializeField] private StatusEffectNonVolatile _statusEffect = null;
    public StatusEffectNonVolatile statusEffect { get => _statusEffect; private set { _statusEffect = value; } }
    public int statusEffectStepCount = 0;
    public int statusEffectLifeTime = 0;
    public float catchRateStatusBonus => statusEffect is null ? 1f : statusEffect.catchRateBonus;
    public bool isFainted => hp < 1;
    public bool isAtFullHP => hp >= maxHp;
    public StageMultiplierOffensive stageAttack;
    public StageMultiplierOffensive stageSpecialAttack;
    public StageMultiplierDefensive stageDefense;
    public StageMultiplierDefensive stageSpecialDefense;
    public StageMultiplier stageSpeed;
    public StageMultiplierOffensive stageAccuracy;
    public StageMultiplierDefensive stageEvasion;

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

    public string Name => nickname is null || nickname.Length < 1 ? SpeciesName : nickname;
    public string SpeciesName => data.fullName.ToUpper();
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
        stageAttack = new StageMultiplierOffensive(statStageMultipliers);
        stageSpecialAttack = new StageMultiplierOffensive(statStageMultipliers);
        stageDefense = new StageMultiplierDefensive(statStageMultipliers);
        stageSpecialDefense = new StageMultiplierDefensive(statStageMultipliers);
        stageSpeed = new StageMultiplier(statStageMultipliers);
        stageAccuracy = new StageMultiplierOffensive(accuracyStageMultipliers);
        stageEvasion = new StageMultiplierDefensive(accuracyStageMultipliers);

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
            Faint();
            return true;
        }
        return false;
    }

    public void Faint()
    {
        hp = 0;
        HealStatusEffect();
    }

    public bool IsImmuneToStatusEffect(StatusEffectNonVolatile statusEffect)
    {
        foreach (PokemonTypeData type in data.pokemonTypes)
            if (statusEffect.immuneTypes.Contains(type))
                return true;
        return false;
    }

    public void InflictStatusEffect(StatusEffectNonVolatile statusEffect)
    {
        this.statusEffect = statusEffect;
        statusEffectLifeTime = UnityEngine.Random.Range(statusEffect.lifetimeRoundsMinimum, statusEffect.lifetimeRoundsMaximum);
        statusEffectStepCount = 0;
    }

    public void InflictDamageOverTime()
    {
        if (statusEffect is null || statusEffect.damageOverTime < 1)
            return;

        statusEffectStepCount += 1;
        if (statusEffectStepCount >= 4)
        {
            statusEffectStepCount = 0;
            InflictDamage(statusEffect.damageOverTime);
        }
    }

    public void HealStatusEffect() => statusEffect = null;

    public void HealFully()
    {
        HealHPFully();

        foreach (Move move in moves)
            move.ReplenishPP();

        HealStatus();
        // TODO: HEal status effects
    }

    public void HealHP(int hp) => this.hp = Math.Min(maxHp, this.hp + hp);
    public void HealHPFully() => hp = maxHp;
    public void HealStatus() => statusEffect = null;

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

    // https://bulbapedia.bulbagarden.net/wiki/Catch_rate#Capture_method_.28Generation_I.29
    public float GetModifiedCatchRate(float bonus)
    {
        return Mathf.Max(
            (3f * maxHp - 2f * hp) * data.catchRate * bonus * catchRateStatusBonus / (3f * maxHp),
            data.catchRate / 3f);
    }

    public override string ToString() => $"{base.ToString()} {Name} {SpeciesName}";
}
