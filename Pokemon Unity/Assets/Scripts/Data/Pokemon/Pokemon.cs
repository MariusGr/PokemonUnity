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
    public int GetAttack(bool criticalHit) => (int)(attackUnmodified * stageAttack.GetMultiplier(criticalHit));
    public int GetSpecialAttack(bool criticalHit) => (int)(specialAttackUnmodified * stageSpecialAttack.GetMultiplier(criticalHit));
    public int GetDefense(bool criticalHit) => (int)(defenseUnmodified * stageDefense.GetMultiplier(criticalHit));
    public int GetSpecialDefense(bool criticalHit) => (int)(specialDefenseUnmodified * stageSpecialDefense.GetMultiplier(criticalHit));
    // TODO same for volatile status
    public int speed
        => (int)(speedUnmodified * stageSpeed.GetMultiplier() * (statusEffectNonVolatile is null ? 1f : statusEffectNonVolatile.data.statModifierSpeedRelative));

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
    [SerializeField] private StatusEffect _statusEffectNonVolatile = null;
    public StatusEffect statusEffectNonVolatile { get => _statusEffectNonVolatile; private set { _statusEffectNonVolatile = value; } }
    public List<StatusEffect> statusEffectsVolatile = new List<StatusEffect>();
    public float catchRateStatusBonus
    {
        get
        {
            float catchRateBonus = statusEffectNonVolatile is null ? 1f : statusEffectNonVolatile.data.catchRateBonus;
            foreach (StatusEffect s in statusEffectsVolatile)
                catchRateBonus *= s.data.catchRateBonus;
            return catchRateBonus;
        }
    }

    public bool isFainted => hp < 1;
    public bool isAtFullHP => hp >= maxHp;
    public bool hasNonVolatileStatusEffect => !(statusEffectNonVolatile is null);
    public bool hasVolatileStatusEffects => statusEffectsVolatile.Count > 0;
    public StageMultiplierOffensive stageAttack;
    public StageMultiplierOffensive stageSpecialAttack;
    public StageMultiplierDefensive stageDefense;
    public StageMultiplierDefensive stageSpecialDefense;
    public StageMultiplier stageSpeed;
    public StageMultiplierOffensive stageAccuracy;
    public StageMultiplierDefensive stageEvasion;
    private Dictionary<Stat, StageMultiplier> statToStage;

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

        statToStage = new Dictionary<Stat, StageMultiplier>()
        {
            { Stat.Attack, stageAttack },
            { Stat.SpecialAttack, stageSpecialAttack },
            { Stat.Defense, stageDefense },
            { Stat.SpecialDefense, stageSpecialDefense },
            { Stat.Speed, stageSpeed },
            { Stat.Accuracy, stageAccuracy },
            { Stat.Evasion, stageEvasion },
        };

        statusEffectsVolatile = new List<StatusEffect>();

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

    public int InflictStatModifier(Stat stat, int amount)
    {
        StageMultiplier stage = statToStage[stat];
        int result = 0;
        if (stage.stage + amount > 6)
            result = 1;
        if (stage.stage + amount < -6)
            result = - 1;

        stage.stage = Math.Min(6, Math.Max(-6, stage.stage + amount));
        return result;
    }

    public void ResetStatModifiers()
    {
        foreach (StageMultiplier stage in statToStage.Values)
            stage.stage = 0;
    }

    public void Faint()
    {
        hp = 0;
        ResetStatModifiers();
        HealStatusAllEffects();
    }

    public bool NonVolatileStatusWillNotHaveEffect(StatusEffectNonVolatileData statusEffect)
        => hasNonVolatileStatusEffect || IsImmuneToStatusEffect(statusEffect);
    public bool VolatileStatusWillNotHaveEffect(StatusEffectVolatileData statusEffect)
        => HasStatusEffectVolatile(statusEffect) || IsImmuneToStatusEffect(statusEffect);

    public bool IsImmuneToStatusEffect(StatusEffectData statusEffect)
    {
        foreach (PokemonTypeData type in data.pokemonTypes)
            if (statusEffect.immuneTypes.Contains(type))
                return true;
        return false;
    }

    private StatusEffect FindVolatileStatusEffect(StatusEffectData statusEffect)
    {
        foreach (StatusEffect s in statusEffectsVolatile)
            if (s.data == statusEffect)
                return s;
        return null;
    }

    public bool HasStatusEffectVolatile(StatusEffectData statusEffect)
    {
        if (FindVolatileStatusEffect(statusEffect) is null)
            return false;
        return true;
    }

    public void InflictStatusEffect(StatusEffectData statusEffect)
    {
        if (statusEffect.isVolatile)
            InflictStatusVolatileEffect((StatusEffectVolatileData)statusEffect);
        else if (statusEffect.isNonVolatile)
            InflictStatusNonVolatileEffect((StatusEffectNonVolatileData)statusEffect);
    }

    public void InflictStatusNonVolatileEffect(StatusEffectNonVolatileData statusEffect) => statusEffectNonVolatile = new StatusEffect(statusEffect);
    public void InflictStatusVolatileEffect(StatusEffectVolatileData statusEffect) => statusEffectsVolatile.Add(new StatusEffect(statusEffect));

    public void InflictDamageOverTime()
    {
        if (statusEffectNonVolatile is null || statusEffectNonVolatile.data.damageOverTime < 1)
            return;

        // TODO: volatile status effects
        if (statusEffectNonVolatile.OverTimeDamageTick())
            InflictDamage(statusEffectNonVolatile.data.damageOverTime);
    }

    public void HealStatusEffectNonVolatile(StatusEffectData statusEffect)
        => statusEffectNonVolatile = statusEffectNonVolatile.data == statusEffect ? null : statusEffectNonVolatile;
    public void HealAllStatusEffectsNonVolatile()
        => statusEffectNonVolatile = null;
    public void HealAllVolatileStatusEffects() => statusEffectsVolatile = new List<StatusEffect>();

    public StatusEffect HealStatusEffectVolatile(StatusEffectVolatileData statusEffect)
    {
        StatusEffect s = FindVolatileStatusEffect(statusEffect);
        if (!(s is null))
            HealStatusEffectVolatile(s);
        return s;
    }

    public void HealStatusEffectVolatile(StatusEffect statusEffect)
    {
        if (statusEffectsVolatile.Contains(statusEffect))
            statusEffectsVolatile.Remove(statusEffect);
    }

    public void HealStatusAllEffects()
    {
        statusEffectNonVolatile = null;
        HealAllVolatileStatusEffects();
    }

    public void HealFully()
    {
        HealHPFully();

        foreach (Move move in moves)
            move.ReplenishPP();

        HealStatusAllEffects();
        // TODO: HEal status effects
    }

    public void HealHP(int hp) => this.hp = Math.Min(maxHp, this.hp + hp);
    public void HealHPFully() => hp = maxHp;
    public void HealStatus(StatusEffect statusEffect)
    {
        if (statusEffect.data.isVolatile)
            HealStatusEffectVolatile(statusEffect);
        else if (statusEffect.data.isNonVolatile)
            HealStatusEffectNonVolatile(statusEffect.data);
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

    // https://bulbapedia.bulbagarden.net/wiki/Catch_rate#Capture_method_.28Generation_I.29
    public float GetModifiedCatchRate(float bonus)
    {
        return Mathf.Max(
            (3f * maxHp - 2f * hp) * data.catchRate * bonus * catchRateStatusBonus / (3f * maxHp),
            data.catchRate / 3f);
    }

    public override string ToString() => $"{base.ToString()} {Name} {SpeciesName}";
}
