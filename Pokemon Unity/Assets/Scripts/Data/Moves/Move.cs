using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;
using AYellowpaper;

[Serializable]
public class Move : IMove
{
    [field: SerializeField] public IMoveData Data { get; private set; }
    [field: SerializeField] public IPokemon pokemon { get; private set; }
    [field: SerializeField] public int Index { get; set; }
    [field: SerializeField] public int Pp { get; private set; }
    [field: SerializeField] public bool IsBlocked { get; private set; }

    public bool IsUsable => Pp > 0 && !IsBlocked;

    public Move(IMoveData data, int index, Pokemon pokemon) : this((MoveData)data, index, pokemon) { }

    public Move(MoveData data, int index, Pokemon pokemon)
    {
        Index = index;
        Data = data;
        this.pokemon = pokemon;
        Pp = data.MaxPP;
    }

    public Move(JSONNode json, int index, Pokemon pokemon)
    {
        Index = index;
        Data = (MoveData)BaseScriptableObject.Get(json["data"]);
        Pp = json["pp"];
        this.pokemon = pokemon;
    }

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();
        json.Add("data", Data.Id);
        json.Add("pp", Pp);
        return json;
    }

    public void SetPokemon(IPokemon pokemon) => this.pokemon = pokemon;

    // https://bulbapedia.bulbagarden.net/wiki/Accuracy
    public bool TryHit(IPokemon attacker, IPokemon target, out FailReason failReason)
    {
        failReason = FailReason.None;

        if (
            Data.OnlyInflictsNonVolatileStatusEffectOnTarget && target.NonVolatileStatusWillNotHaveEffect(Data.StatusNonVolatileInflictedTarget.Value) ||
            Data.OnlyInflictsVolatileStatusOnTarget && target.VolatileStatusWillNotHaveEffect(Data.StatusVolatileInflictedTarget.Value) ||
            Data.OnlyInflictsBothStatusEffectsOnTarget &&
            target.NonVolatileStatusWillNotHaveEffect(Data.StatusNonVolatileInflictedTarget.Value) &&
            target.VolatileStatusWillNotHaveEffect(Data.StatusVolatileInflictedTarget.Value)
        )
        {
            failReason = FailReason.NoEffect;
            return false;
        }

        if (Data.Accuracy < 0)
            return true;

        int accuracyModified = (int)(Data.Accuracy * attacker.StageAccuracy.GetMultiplier());
        Debug.Log($"accuracy: {Data.Accuracy}, accuracyModified: {accuracyModified}");

        if (UnityEngine.Random.Range(0, 100) < accuracyModified)
            return true;

        failReason = FailReason.Miss;
        return false;
    }

    public int GetDamageAgainst(IPokemon attacker, IPokemon target, out bool critcal, out Effectiveness effectiveness)
    {
        critcal = UnityEngine.Random.value * 255f <= attacker.Data.Speed / 2f;
        float criticalFactor = 1f;
        if (critcal)
            criticalFactor = 1.5f;

        float stab = attacker.MatchesType(Data.PokeType.Value) ? 1.5f : 1f;

        float effectivenessFactor = Data.PokeType.Value.GetEffectiveness(target);
        if (effectivenessFactor <= 0)
            effectiveness = Effectiveness.Ineffecitve;
        else if (effectivenessFactor < 1)
            effectiveness = Effectiveness.Weak;
        else if (effectivenessFactor > 1)
            effectiveness = Effectiveness.Strong;
        else
            effectiveness = Effectiveness.Normal;

        int targetDefense = Data.Category.Value.IsSpecial ? target.GetSpecialDefense(critcal) : target.GetDefense(critcal);
        int attackerAttack = Data.Category.Value.IsSpecial ? attacker.GetSpecialAttack(critcal) : attacker.GetAttack(critcal);

        int damage = (int)Mathf.Max(
            0, Mathf.Floor(
                ((.4f * attacker.Level + 2) *
                Data.Power *
                (attacker.StatusEffectNonVolatile is null ? 1f : attacker.StatusEffectNonVolatile.Data.DamageModifierRelative) *
                attackerAttack /
                targetDefense / 50f + 2f) *
                criticalFactor * stab * effectivenessFactor)
        );
        Debug.Log($"level: {attacker.Level}, power: {Data.Power}, attack: {attackerAttack}," +
            $"defense: {targetDefense}, critical: {criticalFactor}, stab: {stab}, effectiveness: {effectivenessFactor}, total: {damage}");

        if (damage < 1)
            effectiveness = Effectiveness.Ineffecitve;

        return damage;
    }

    public bool IsFaster(IMove other)
    {
        Debug.Log($"Faster? {pokemon}: {pokemon.Speed} vs. {other.pokemon}: {other.pokemon.Speed}");
        if (pokemon.Speed != other.pokemon.Speed)
            return pokemon.Speed > other.pokemon.Speed;
        return UnityEngine.Random.value > .5f;
    }

    public void DecrementPP() => Pp = Data.MaxPP > 0 ? Math.Max(0, Pp - 1) : Pp;
    public void ReplenishPP() => Pp = Data.MaxPP;
}
