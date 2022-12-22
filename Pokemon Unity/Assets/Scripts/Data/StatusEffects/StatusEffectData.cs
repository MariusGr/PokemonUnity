using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectData : ScriptableObject
{
    public string nameSubject = "";
    public string inflictionText = "";
    public string notificationPerRoundText = "";
    public string effectPerRoundText = "";
    public string endOfLifeText = "";
    public string healText = "";
    public bool livesForever = true;
    public int lifetimeRoundsMinimum = 0;
    public int lifetimeRoundsMaximum = 0;
    public bool takesEffectOnceWhenLifeTimeEnded = false;
    public bool takesEffectBeforeMoves = false;
    public int damagePerRoundAbsolute = 0;
    public float damagePerRoundRelativeToMaxHp = 0;
    public int powerOfSelfInflictedDamagePerRound = 0;
    public int damageOverTime = 0;
    public bool preventsMove = true;
    public float chance = 1f;
    public float statModifierSpeedRelative = 1f;
    public float damageModifierRelative = 1f;
    public float catchRateBonus = 1f;
    public List<PokemonTypeData> immuneTypes;

    public bool isVolatile => GetType() == typeof(StatusEffectVolatileData);
    public bool isNonVolatile => GetType() == typeof(StatusEffectNonVolatileData);

    public int GetDamageAgainstSelf(Pokemon pokemon)
        => (int)Mathf.Max(0, Mathf.Floor(
                    ((.4f * pokemon.level + 2) *
                    powerOfSelfInflictedDamagePerRound *
                    (pokemon.statusEffectNonVolatile is null ? 1f : pokemon.statusEffectNonVolatile.data.damageModifierRelative) *
                    pokemon.GetAttack(false) /
                    pokemon.GetDefense(false) / 50f + 2f))
                );
}
