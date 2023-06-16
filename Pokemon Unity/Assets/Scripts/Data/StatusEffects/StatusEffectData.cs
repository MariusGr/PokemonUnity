using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectData : BaseScriptableObject, IStatusEffectData
{
    [field: SerializeField] public string NameSubject { get; private set; } = "";
    [field: SerializeField] public string InflictionText { get; private set; } = "";
    [field: SerializeField] public string NotificationPerRoundText { get; private set; } = "";
    [field: SerializeField] public string EffectPerRoundText { get; private set; } = "";
    [field: SerializeField] public string WaitForEffectText { get; private set; } = "";
    [field: SerializeField] public string EndOfLifeText { get; private set; } = "";
    [field: SerializeField] public string HealText { get; private set; } = "";
    [field: SerializeField] public AudioClip Sound { get; private set; }
    [field: SerializeField] public bool LivesForever { get; private set; } = true;
    [field: SerializeField] public int LifetimeRoundsMinimum { get; private set; } = 0;
    [field: SerializeField] public int LifetimeRoundsMaximum { get; private set; } = 0;
    [field: SerializeField] public bool TakesEffectOnceWhenLifeTimeEnded { get; private set; } = false;
    [field: SerializeField] public bool TakesEffectBeforeMoves { get; private set; } = false;
    [field: SerializeField] public int DamagePerRoundAbsolute { get; private set; } = 0;
    [field: SerializeField] public float DamagePerRoundRelativeToMaxHp { get; private set; } = 0;
    [field: SerializeField] public int PowerOfSelfInflictedDamagePerRound { get; private set; } = 0;
    [field: SerializeField] public int DamageOverTime { get; private set; } = 0;
    [field: SerializeField] public bool PreventsMove { get; private set; } = true;
    [field: SerializeField] public float Chance { get; private set; } = 1f;
    [field: SerializeField] public float StatModifierSpeedRelative { get; private set; } = 1f;
    [field: SerializeField] public float DamageModifierRelative { get; private set; } = 1f;
    [field: SerializeField] public float CatchRateBonus { get; private set; } = 1f;
    [field: SerializeField] public List<IPokemonTypeData> ImmuneTypes { get; private set; }

    public bool IsVolatile => GetType() == typeof(StatusEffectVolatileData);
    public bool IsNonVolatile => GetType() == typeof(StatusEffectNonVolatileData);

    public int GetDamageAgainstSelf(IPokemon pokemon)
        => PowerOfSelfInflictedDamagePerRound < 1 ? 0 : (int)Mathf.Max(0, Mathf.Floor(
                    (.4f * pokemon.Level + 2) *
                    PowerOfSelfInflictedDamagePerRound *
                    (pokemon.StatusEffectNonVolatile is null ? 1f : pokemon.StatusEffectNonVolatile.Data.DamageModifierRelative) *
                    pokemon.GetAttack(false) /
                    pokemon.GetDefense(false) / 50f + 2f)
                );
}
