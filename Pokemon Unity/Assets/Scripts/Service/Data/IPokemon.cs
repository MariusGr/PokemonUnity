using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;

public interface IPokemon
{
    public class StageMultiplier
    {
        private readonly float[] multipliers;
        protected int index;
        public int Stage
        {
            get => IndexToStage(index);
            set => index = StageToIndex(value);
        }
        protected float Multiplier => multipliers[index];

        public StageMultiplier(float[] multipliers)
        {
            index = 6;
            this.multipliers = multipliers;
        }

        public float GetMultiplier() => Multiplier;
        public virtual float GetMultiplier(bool criticalHit) => Multiplier;
        private int StageToIndex(int stage) => Math.Max(0, Math.Min(multipliers.Length, stage + 6));
        private int IndexToStage(int index) => index - 6;
        protected float GetMultiplier(int stage) => multipliers[StageToIndex(stage)];
        //public float GetMultiplier(int substractStage = 0) => multipliers[Math.Max(0, Math.Min(multipliers.Length, stage + substractStage + 6))];
    }

    public class StageMultiplierOffensive : StageMultiplier
    {
        public StageMultiplierOffensive(float[] multipliers) : base(multipliers) { }
        public override float GetMultiplier(bool criticalHit) => criticalHit ? GetMultiplier(Math.Max(0, Stage)) : Multiplier;
    }

    public class StageMultiplierDefensive : StageMultiplier
    {
        public StageMultiplierDefensive(float[] multipliers) : base(multipliers) { }
        public override float GetMultiplier(bool criticalHit) => criticalHit ? GetMultiplier(Math.Min(0, Stage)) : Multiplier;
    }

    public string Name { get; }
    public string Nickname { get; }
    public string SpeciesName { get; }
    public int Level { get; }
    public List<IMove> Moves { get; }
    public ICharacterData Character { get; }
    public bool IsFainted { get; }
    public IPokemonData Data { get; }

    public DateTime MetDate { get; set; }
    public string MetMap { get; set; }
    public string MetLevel { get; set; }

    public bool IsAtFullHP { get; }
    public bool HasNonVolatileStatusEffect { get; }
    public bool HasVolatileStatusEffects { get; }
    public IStatusEffect StatusEffectNonVolatile { get; }
    public List<IStatusEffect> StatusEffectsVolatile { get; }

    public StageMultiplierOffensive StageAttack { get; }
    public StageMultiplierOffensive StageSpecialAttack { get; }
    public StageMultiplierDefensive StageDefense { get; }
    public StageMultiplierDefensive StageSpecialDefense { get; }
    public StageMultiplier StageSpeed { get; }
    public StageMultiplierOffensive StageAccuracy { get; }
    public StageMultiplierDefensive StageEvasion { get; }

    public int Speed { get; }
    public int AttackUnmodified { get; }
    public int SpecialAttackUnmodified { get; }
    public int DefenseUnmodified { get; }
    public int SpecialDefenseUnmodified { get; }
    public int SpeedUnmodified { get; }
    public int MaxHp { get; }

    public float HpNormalized { get; }
    public float XpNormalized { get; }

    public int GetAttack(bool criticalHit);
    public int GetSpecialAttack(bool criticalHit);
    public int GetDefense(bool criticalHit);
    public int GetSpecialDefense(bool criticalHit);

    public IPokemon GetEvolvedVersion();

    public bool InflictDamage(int damage);
    public int InflictStatModifier(Stat stat, int amount);
    public void ResetStatModifiers();
    public void ResetWaitingVolatileStatusEffects();
    public void ResetWaitingNonVolatileStatusEffects();
    public void ResetWaitingStatusEffects();
    public void Faint();
    public bool NonVolatileStatusWillNotHaveEffect(IStatusEffectNonVolatileData statusEffect);
    public bool VolatileStatusWillNotHaveEffect(IStatusEffectVolatileData statusEffect);
    public bool IsImmuneToStatusEffect(IStatusEffectData statusEffect);
    public bool HasStatusEffectVolatile(IStatusEffectData statusEffect);
    public void InflictWaitingStatusEffect(IWaitingStatusEffect statusEffect);
    public IStatusEffectData GetNextVolatileStatusEffectFromWaitingList();
    public IStatusEffectData GetNextNonVolatileStatusEffectFromWaitingList();
    public void InflictStatusEffect(IStatusEffectData statusEffect);
    public void InflictStatusNonVolatileEffect(JSONNode json);
    public void InflictStatusNonVolatileEffect(IStatusEffectNonVolatileData statusEffect);
    public void InflictStatusVolatileEffect(IStatusEffectVolatileData statusEffect);
    public void InflictDamageOverTime();
    public void HealStatusEffectNonVolatile(IStatusEffectData statusEffect);
    public void HealAllStatusEffectsNonVolatile();
    public void HealAllVolatileStatusEffects();
    public IStatusEffect HealStatusEffectVolatile(IStatusEffectVolatileData statusEffect);
    public void HealStatusEffectVolatile(IStatusEffect statusEffect);
    public void HealStatusAllEffects();
    public void HealFully();
    public void HealHP(int hp);
    public void HealHPFully();
    public void HealStatus(IStatusEffect statusEffect);
    public int GetXPGainedFromFaint(bool opponentIsWild);
    public int GainXP(int xp);
    public void GrowLevel();
    public bool IsLeveledToMax();
    public void ReplaceMove(IMove move, IMoveData newMove);
    public void AddMove(JSONNode json);
    public void AddMove(IMoveData moveData);
    public void SwapMoves(IMove move1, IMove move2);
    public bool HasUsableMoves();
    public float GetModifiedCatchRate(float bonus);
    public bool MatchesType(IPokemonTypeData type);
}
