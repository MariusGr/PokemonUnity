using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[Serializable]
public class Pokemon : IPokemon
{
    // TODO Wrapper class specifically for in-battle useage with stat modifiers, volatile status effects and waiting status effects

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

    [field: SerializeField] public IPokemonData Data { get; private set; }
    [field: SerializeField] public ICharacterData Character { get; private set; }
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public string Nickname { get; private set; } = null;

    [field: SerializeField] public DateTime MetDate { get; set; }
    [field: SerializeField] public string MetMap { get; set; } = "Entonhausen";
    [field: SerializeField] public string MetLevel { get; set; }

    [field: SerializeField] public int Level { get; private set; }
    public int GetAttack(bool criticalHit) => (int)(AttackUnmodified * stageAttack.GetMultiplier(criticalHit));
    public int GetSpecialAttack(bool criticalHit) => (int)(SpecialAttackUnmodified * stageSpecialAttack.GetMultiplier(criticalHit));
    public int GetDefense(bool criticalHit) => (int)(DefenseUnmodified * stageDefense.GetMultiplier(criticalHit));
    public int GetSpecialDefense(bool criticalHit) => (int)(SpecialDefenseUnmodified * stageSpecialDefense.GetMultiplier(criticalHit));
    // TODO same for volatile status
    public int Speed
        => (int)(SpeedUnmodified * stageSpeed.GetMultiplier() * (StatusEffectNonVolatile is null ? 1f : StatusEffectNonVolatile.Data.StatModifierSpeedRelative));

    public int AttackUnmodified => BasteStatToStat(Data.Attack);
    public int SpecialAttackUnmodified => BasteStatToStat(Data.SpecialAttack);
    public int DefenseUnmodified => BasteStatToStat(Data.Defense);
    public int SpecialDefenseUnmodified => BasteStatToStat(Data.SpecialDefense);
    public int SpeedUnmodified => BasteStatToStat(Data.Speed);
    public int MaxHp => BasteStatToStat(Data.MaxHp);

    [field: SerializeField] public List<IMove> Moves { get; private set; }
    public int hp;
    public int xp;
    public int xpNeededForNextLevel => Data.GetXPForLevel(Level + 1);
    [field: SerializeField] public IStatusEffect StatusEffectNonVolatile { get; private set; }
    [field: SerializeField] public List<IStatusEffect> StatusEffectsVolatile { get; private set; } = new List<IStatusEffect>();
    public List<IWaitingStatusEffect> waitingStatusEffectsVolatile = new List<IWaitingStatusEffect>();
    public List<IWaitingStatusEffect> waitingStatusEffectsNonVolatile = new List<IWaitingStatusEffect>();
    public float catchRateStatusBonus
    {
        get
        {
            float catchRateBonus = StatusEffectNonVolatile is null ? 1f : StatusEffectNonVolatile.Data.CatchRateBonus;
            foreach (IStatusEffect s in StatusEffectsVolatile)
                catchRateBonus *= s.Data.CatchRateBonus;
            return catchRateBonus;
        }
    }

    public bool IsFainted => hp < 1;
    public bool IsAtFullHP => hp >= MaxHp;
    public bool HasNonVolatileStatusEffect => !(StatusEffectNonVolatile is null);
    public bool HasVolatileStatusEffects => StatusEffectsVolatile.Count > 0;
    public IPokemon.StageMultiplierOffensive stageAttack;
    public IPokemon.StageMultiplierOffensive stageSpecialAttack;
    public IPokemon.StageMultiplierDefensive stageDefense;
    public IPokemon.StageMultiplierDefensive stageSpecialDefense;
    public IPokemon.StageMultiplier stageSpeed;
    public IPokemon.StageMultiplierOffensive stageAccuracy;
    public IPokemon.StageMultiplierDefensive stageEvasion;
    private Dictionary<Stat, IPokemon.StageMultiplier> statToStage;

    public IPokemon.StageMultiplierOffensive StageAttack => stageAttack;
    public IPokemon.StageMultiplierOffensive StageSpecialAttack => stageSpecialAttack;
    public IPokemon.StageMultiplierDefensive StageDefense => stageDefense;
    public IPokemon.StageMultiplierDefensive StageSpecialDefense => stageSpecialDefense;
    public IPokemon.StageMultiplier StageSpeed => stageSpeed;
    public IPokemon.StageMultiplierOffensive StageAccuracy => stageAccuracy;
    public IPokemon.StageMultiplierDefensive StageEvasion => stageEvasion;

    public float HpNormalized => (float)hp / MaxHp;
    public float XpNormalized
    {
        get
        {
            int xpMilestoneCurrent = Data.GetXPForLevel(Level);
            Debug.Log($"current {xpMilestoneCurrent} next {xpNeededForNextLevel}, xp {xp} norm {(xp - xpMilestoneCurrent) / (xpNeededForNextLevel - xpMilestoneCurrent)}");

            return Math.Min(1f, (xp - xpMilestoneCurrent) / (float)(xpNeededForNextLevel - xpMilestoneCurrent));
        }
    }

    public string Name => Nickname is null || Nickname.Length < 1 ? SpeciesName : Nickname;
    public string SpeciesName => Data.FullName.ToUpper();
    public Gender gender;

    private int BasteStatToStat(int baseStat) => baseStat + baseStat * Level / 50;

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();
        json.Add("data", Data.Id);
        json.Add("metDate", MetDate.ToString());
        json.Add("metMap", MetMap);
        json.Add("metLevel", MetLevel);
        json.Add("level", Level);
        json.Add("moves", MovesToJSON());
        json.Add("hp", hp);
        json.Add("xp", xp);
        json.Add("gender", gender.Id);
        json.Add("statusEffectNonVolatile", HasNonVolatileStatusEffect ? StatusEffectNonVolatile.ToJSON() : null);
        return json;
    }

    private JSONArray MovesToJSON()
    {
        JSONArray json = new JSONArray();

        foreach (Move move in Moves)
            json.Add(move.ToJSON());

        return json;
    }

    private void LoadMovesFromJSON(JSONArray json)
    {
        Moves = new List<IMove>();
        foreach (JSONNode moveJSON in json)
            AddMove(moveJSON);
    }

    public Pokemon(EncounterPokemon encounterPokemonData) :
        this(encounterPokemonData.data, UnityEngine.Random.Range(encounterPokemonData.minLevel, encounterPokemonData.maxLevel), null)
    {
        Initialize(null);
        LoadDefault();
    }

    public Pokemon(IPokemonData data, int level, ICharacterData character) : this((PokemonData)data, level, (CharacterData)character) { }
    public Pokemon(PokemonData data, int level, CharacterData character)
    {
        Data = data;
        this.Level = level;
        Initialize(character);
    }

    public Pokemon(JSONNode json, CharacterData character)
    {
        Data = (PokemonData)BaseScriptableObject.Get(json["data"]);
        Level = json["level"];
        MetDate = DateTime.Parse(json["metDate"]);
        MetLevel = json["metLevel"];
        MetMap = json["metMap"];
        LoadMovesFromJSON((JSONArray)json["moves"]);
        hp = json["hp"];
        xp = json["xp"];
        gender = (Gender)BaseScriptableObject.Get(json["gender"]);
        JSONNode statusEffectJSON = json["statusEffectNonVolatile"];
        if(!(statusEffectJSON is null) && statusEffectJSON.HasKey("data")) InflictStatusNonVolatileEffect(statusEffectJSON);

        Initialize(character);
    }

    public void Initialize(CharacterData character)
    {
        this.Character = character;
        stageAttack = new IPokemon.StageMultiplierOffensive(statStageMultipliers);
        stageSpecialAttack = new IPokemon.StageMultiplierOffensive(statStageMultipliers);
        stageDefense = new IPokemon.StageMultiplierDefensive(statStageMultipliers);
        stageSpecialDefense = new IPokemon.StageMultiplierDefensive(statStageMultipliers);
        stageSpeed = new IPokemon.StageMultiplier(statStageMultipliers);
        stageAccuracy = new IPokemon.StageMultiplierOffensive(accuracyStageMultipliers);
        stageEvasion = new IPokemon.StageMultiplierDefensive(accuracyStageMultipliers);

        statToStage = new Dictionary<Stat, IPokemon.StageMultiplier>()
        {
            { Stat.Attack, stageAttack },
            { Stat.SpecialAttack, stageSpecialAttack },
            { Stat.Defense, stageDefense },
            { Stat.SpecialDefense, stageSpecialDefense },
            { Stat.Speed, stageSpeed },
            { Stat.Accuracy, stageAccuracy },
            { Stat.Evasion, stageEvasion },
        };

        StatusEffectsVolatile = new List<IStatusEffect>();
    }

    public void LoadDefault()
    {
        Moves = new List<IMove>();
        hp = MaxHp;

        int added = 0;
        for (int i = Data.LevelToMoveDataMap.keys.Count - 1; i > -1; i--)
        {
            int key = Data.LevelToMoveDataMap.keys[i];
            if (key <= Level)
            {
                AddMove(Data.LevelToMoveDataMap[key]);
                added++;
                if (added > 3)
                    break;
            }
        }

        if (gender is null) gender = Gender.GetRandomGender();
        xp = Data.GetXPForLevel(Level);
    }

    public void AddMove(JSONNode json) => Moves.Add(new Move(json, Moves.Count, this));
    public void AddMove(IMoveData moveData) => Moves.Add(new Move(moveData, Moves.Count, this));
    public void ReplaceMove(IMove move, IMoveData newMove) => Moves[move.Index] = new Move(newMove, move.Index, this);

    public void SwapMoves(IMove move1, IMove move2)
    {
        if (move1 == move2)
            return;
        int newIndex1 = move2.Index;
        int newIndex2 = move1.Index;
        Moves[newIndex1] = move1;
        Moves[newIndex2] = move2;
        move1.Index = newIndex1;
        move2.Index = newIndex2;
    }

    public bool HasUsableMoves()
    {
        foreach (Move move in Moves)
            if (move.IsUsable)
                return true;
        return false;
    }

    public bool MatchesType(IPokemonTypeData type)
    {
        foreach (IPokemonTypeData myType in Data.PokemonTypes)
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
        IPokemon.StageMultiplier stage = statToStage[stat];
        int result = 0;
        if (stage.Stage + amount > 6)
            result = 1;
        if (stage.Stage + amount < -6)
            result = - 1;

        stage.Stage = Math.Min(6, Math.Max(-6, stage.Stage + amount));
        return result;
    }

    public void ResetStatModifiers()
    {
        foreach (IPokemon.StageMultiplier stage in statToStage.Values)
            stage.Stage = 0;
    }

    public void ResetWaitingVolatileStatusEffects() => waitingStatusEffectsVolatile = new List<IWaitingStatusEffect>();
    public void ResetWaitingNonVolatileStatusEffects() => waitingStatusEffectsNonVolatile = new List<IWaitingStatusEffect>();

    public void ResetWaitingStatusEffects()
    {
        ResetWaitingNonVolatileStatusEffects();
        ResetWaitingVolatileStatusEffects();
    }

    public void Faint()
    {
        hp = 0;
        ResetStatModifiers();
        HealStatusAllEffects();
    }

    public bool NonVolatileStatusWillNotHaveEffect(IStatusEffectNonVolatileData statusEffect)
        => HasNonVolatileStatusEffect || IsImmuneToStatusEffect(statusEffect);
    public bool VolatileStatusWillNotHaveEffect(IStatusEffectVolatileData statusEffect)
        => HasStatusEffectVolatile(statusEffect) || IsImmuneToStatusEffect(statusEffect);

    public bool IsImmuneToStatusEffect(IStatusEffectData statusEffect)
    {
        foreach (PokemonTypeData type in Data.PokemonTypes)
            if (statusEffect.ImmuneTypes.Contains(type))
                return true;
        return false;
    }

    private IStatusEffect FindVolatileStatusEffect(IStatusEffectData statusEffect)
    {
        foreach (IStatusEffect s in StatusEffectsVolatile)
            if (s.Data == statusEffect)
                return s;
        return null;
    }

    public bool HasStatusEffectVolatile(IStatusEffectData statusEffect)
    {
        if (FindVolatileStatusEffect(statusEffect) is null)
            return false;
        return true;
    }

    public void InflictWaitingStatusEffect(IWaitingStatusEffect statusEffect)
    {
        if (statusEffect.Data.IsNonVolatile)
            waitingStatusEffectsNonVolatile.Add(statusEffect);
        else
            waitingStatusEffectsVolatile.Add(statusEffect);
    }

    public IStatusEffectData GetNextVolatileStatusEffectFromWaitingList()
    {
        IStatusEffectData statusEffect = GetNextStatusEffectFromWaitingList(waitingStatusEffectsVolatile);

        if (!(statusEffect is null))
            ResetWaitingVolatileStatusEffects();

        return statusEffect;
    }

    public IStatusEffectData GetNextNonVolatileStatusEffectFromWaitingList()
    {
        IStatusEffectData statusEffect = GetNextStatusEffectFromWaitingList(waitingStatusEffectsNonVolatile);

        if (!(statusEffect is null))
            ResetWaitingNonVolatileStatusEffects();

        return statusEffect;
    }

    private IStatusEffectData GetNextStatusEffectFromWaitingList(List<IWaitingStatusEffect> waitingStatusEffects)
    {
        IStatusEffectData statusEffect = null;
        foreach(WaitingStatusEffect effect in waitingStatusEffects)
        {
            effect.waitTimeRounds--;
            if (effect.waitTimeRounds < 1)
            {
                statusEffect = effect.Data;
                break;
            }
        }

        return statusEffect;
    }

    public void InflictStatusEffect(IStatusEffectData statusEffect)
    {
        if (statusEffect.IsVolatile)
            InflictStatusVolatileEffect((StatusEffectVolatileData)statusEffect);
        else if (statusEffect.IsNonVolatile)
            InflictStatusNonVolatileEffect((StatusEffectNonVolatileData)statusEffect);
    }

    public void InflictStatusNonVolatileEffect(JSONNode json) => StatusEffectNonVolatile = new StatusEffect(json);
    public void InflictStatusNonVolatileEffect(IStatusEffectNonVolatileData statusEffect)
        => StatusEffectNonVolatile = new StatusEffect(statusEffect);
    public void InflictStatusVolatileEffect(IStatusEffectVolatileData statusEffect)
        => StatusEffectsVolatile.Add(new StatusEffect(statusEffect));

    public void InflictDamageOverTime()
    {
        if (StatusEffectNonVolatile is null || StatusEffectNonVolatile.Data.DamageOverTime < 1)
            return;

        // TODO: volatile status effects
         if (StatusEffectNonVolatile.OverTimeDamageTick())
            InflictDamage(StatusEffectNonVolatile.Data.DamageOverTime);
    }

    public void HealStatusEffectNonVolatile(IStatusEffectData statusEffect)
        => StatusEffectNonVolatile = StatusEffectNonVolatile.Data == statusEffect ? null : StatusEffectNonVolatile;
    public void HealAllStatusEffectsNonVolatile()
        => StatusEffectNonVolatile = null;
    public void HealAllVolatileStatusEffects() => StatusEffectsVolatile = new List<IStatusEffect>();

    public IStatusEffect HealStatusEffectVolatile(IStatusEffectVolatileData statusEffect)
    {
        IStatusEffect s = FindVolatileStatusEffect(statusEffect);
        if (!(s is null))
            HealStatusEffectVolatile(s);
        return s;
    }

    public void HealStatusEffectVolatile(IStatusEffect statusEffect)
    {
        if (StatusEffectsVolatile.Contains(statusEffect))
            StatusEffectsVolatile.Remove(statusEffect);
    }

    public void HealStatusAllEffects()
    {
        StatusEffectNonVolatile = null;
        HealAllVolatileStatusEffects();
    }

    public void HealFully()
    {
        HealHPFully();

        foreach (Move move in Moves)
            move.ReplenishPP();

        HealStatusAllEffects();
        // TODO: HEal status effects
    }

    public void HealHP(int hp) => this.hp = Math.Min(MaxHp, this.hp + hp);
    public void HealHPFully() => hp = MaxHp;
    public void HealStatus(IStatusEffect statusEffect)
    {
        if (statusEffect.Data.IsVolatile)
            HealStatusEffectVolatile(statusEffect);
        else if (statusEffect.Data.IsNonVolatile)
            HealStatusEffectNonVolatile(statusEffect.Data);
    }

    // https://bulbapedia.bulbagarden.net/wiki/Experience
    public int GetXPGainedFromFaint(bool opponentIsWild)
    {
        float a = opponentIsWild ? 1f : 1.5f;
        return (int)((Data.BaseXPGain * Level / 7f) * a);
    }

    public int GainXP(int xp) => IsLeveledToMax() ? this.xp = Data.GetXPForLevel(100) : this.xp += xp;
    public void GrowLevel()
    {
        // TODO: learn new moves
        float hpBefore = HpNormalized;
        Level++;
        hp = (int)Mathf.Ceil(hpBefore * MaxHp);
    }

    public bool WillGrowLevel() => xp >= xpNeededForNextLevel && !IsLeveledToMax();
    public bool IsLeveledToMax() => Level >= 100;
    public bool WillEvolve() => Data.EvolutionLevel > 1 && !(Data.Evolution is null) && Level >= Data.EvolutionLevel;

    public IPokemon GetEvolvedVersion()
    {
        Pokemon evolved = new Pokemon(Data.Evolution, Level, Character);
        evolved.xp = xp;
        evolved.hp = (int)(evolved.MaxHp * HpNormalized);
        evolved.gender = gender;
        evolved.Nickname = Nickname;
        evolved.Moves = new List<IMove>();
        foreach (Move m in Moves)
            evolved.Moves.Add(m);
        return evolved;
    }

    // https://bulbapedia.bulbagarden.net/wiki/Catch_rate#Capture_method_.28Generation_I.29
    public float GetModifiedCatchRate(float bonus)
    {
        return Mathf.Max(
            (3f * MaxHp - 2f * hp) * Data.CatchRate * bonus * catchRateStatusBonus / (3f * MaxHp),
            Data.CatchRate / 3f);
    }

    public override string ToString() => $"{base.ToString()} {Name} {SpeciesName}";
}
