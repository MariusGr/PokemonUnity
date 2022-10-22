using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Move
{
    public MoveData data;
    public int index;
    public int pp;
    public bool isBlocked;

    public bool isUsable => pp > 0 && !isBlocked;

    private Pokemon pokemon;

    public Move(MoveData data, int index, Pokemon pokemon)
    {
        this.index = index;
        this.data = data;
        this.pokemon = pokemon;
        pp = data.maxPP;
    }

    public void SetPokemon(Pokemon pokemon) => this.pokemon = pokemon;

    public int GetDamageAgainst(Pokemon attacker, Pokemon target, out bool critcal, out Effectiveness effectiveness)
    {
        critcal = UnityEngine.Random.value * 255f <= attacker.data.speed / 2f;
        float criticalFactor = 1f;
        if (critcal)
            criticalFactor = 1.5f;

        float stab = attacker.MatchesType(data.pokeType) ? 1.5f : 1f;

        float effectivenessFactor = data.pokeType.GetEffectiveness(target);
        if (effectivenessFactor <= 0)
            effectiveness = Effectiveness.Ineffecitve;
        else if (effectivenessFactor < 1)
            effectiveness = Effectiveness.Weak;
        else if (effectivenessFactor > 1)
            effectiveness = Effectiveness.Strong;
        else
            effectiveness = Effectiveness.Normal;

        int damage = (int)Mathf.Max(
            0, Mathf.Floor(
                ((2f * attacker.level + 2) *
                data.power *
                attacker.attack /
                target.defense / 50f + 2f) *
            criticalFactor * stab * effectivenessFactor)
        );
        Debug.Log($"level: {attacker.level}, power: {data.power}, attack: {attacker.attack}," +
            $"defense: {target.defense}, critical: {criticalFactor}, stab: {stab}, effectiveness: {effectivenessFactor}, total: {damage}");

        if (damage < 1)
            effectiveness = Effectiveness.Ineffecitve;

        return damage;
    }

    public bool IsFaster(Move other)
    {
        if (pokemon.speed != other.pokemon.speed)
            return pokemon.speed > other.pokemon.speed;
        return UnityEngine.Random.value > .5f;
    }

    public void DecrementPP()
    {
        if (data.maxPP > 0)
            pp--;
    }
}
