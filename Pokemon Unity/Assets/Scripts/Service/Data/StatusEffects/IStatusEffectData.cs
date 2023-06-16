using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusEffectData
{
    public string Id { get; }
    public string NameSubject { get; }
    public string InflictionText { get; }
    public string NotificationPerRoundText { get; }
    public string EffectPerRoundText { get; }
    public string WaitForEffectText { get; }
    public string EndOfLifeText { get; }
    public string HealText { get; }
    public AudioClip Sound { get; }
    public bool LivesForever { get; }
    public int LifetimeRoundsMinimum { get; }
    public int LifetimeRoundsMaximum { get; }
    public bool TakesEffectOnceWhenLifeTimeEnded { get; }
    public bool TakesEffectBeforeMoves { get; }
    public int DamagePerRoundAbsolute { get; }
    public float DamagePerRoundRelativeToMaxHp { get; }
    public int PowerOfSelfInflictedDamagePerRound { get; }
    public int DamageOverTime { get; }
    public bool PreventsMove { get; }
    public float Chance { get; }
    public float StatModifierSpeedRelative { get; }
    public float DamageModifierRelative { get; }
    public float CatchRateBonus { get; }
    public List<IPokemonTypeData> ImmuneTypes { get; }

    public bool IsVolatile { get; }
    public bool IsNonVolatile { get; }

    public int GetDamageAgainstSelf(IPokemon pokemon);
}
