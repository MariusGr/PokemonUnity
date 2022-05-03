using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Move
{
    public MoveData data;
    public int index;
    public int pp;

    private Pokemon pokemon;

    public Move(Pokemon pokemon, MoveData data, int index)
    {
        this.pokemon = pokemon;
        this.index = index;
        this.data = data;
        pp = data.maxPP;
    }

    public int GetDamageAgainst(Pokemon other, out bool critcal, out Effectiveness effectiveness)
    {
        critcal = Random.Range(0, 2) == 0;
        float criticalFactor = 1f;
        if (critcal)
            criticalFactor = 1.5f;

        float stab = pokemon.MatchesType(data.pokeType) ? 1.5f : 1f;

        float effectivenessFactor = data.pokeType.GetEffectiveness(other);
        if (effectivenessFactor < 0)
            effectiveness = Effectiveness.Ineffecitve;
        else if (effectivenessFactor < 1)
            effectiveness = Effectiveness.Weak;
        else if (effectivenessFactor > 1)
            effectiveness = Effectiveness.Strong;
        else
            effectiveness = Effectiveness.Normal;

        int damage = (int)Mathf.Max(0, Mathf.Floor(((2f * pokemon.level + 2) * data.power * pokemon.attack / other.defense / 50f + 2f) * criticalFactor * stab * effectivenessFactor)));

        if (damage < 1)
            effectiveness = Effectiveness.Ineffecitve;

        return damage;
    }
}
