using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;

public interface IItemData : IInstanceWithId
{
    string Name { get; }
    ItemCategory Category { get; }
    float Price { get; }
    Sprite Icon { get; }
    string Details { get; }
    string Description { get; }
    InterfaceReference<IMoveData, ScriptableObject> MoveLearned { get; }
    bool Stacks { get; }
    bool Consumable { get; }
    bool UsableOnBattleOpponent { get; }
    bool CatchesPokemon { get; }
    float CatchRateBonus { get; }
    bool Revives { get; }
    bool HealsHPFully { get; }
    int HpHealed { get; }
    bool HealsAllStatusEffectsNonVolatile { get; }
    bool HealsAllStatusEffectsVolatile { get; }
    InterfaceReference<IStatusEffectNonVolatileData, ScriptableObject> NonVolatileStatusHealed { get; }
    InterfaceReference<IStatusEffectVolatileData, ScriptableObject> VolatileStatusHealed { get; }
    bool CanBeUsedOnOwnPokemon { get; }
    bool HealsHP { get; }
    bool HealsHPOnly { get; }
    bool HealsStatusEffectsNonVolatile { get; }
    bool HealsStatusEffectsNonVolatileOnly { get; }
    bool HealsStatusEffectsVolatile { get; }
    bool HealsStatusEffectsVolatileOnly { get; }

    bool HealsStatusEffectNonVolatile(IStatusEffect statusEffect);
    bool HealsStatusEffectVolatile(List<IStatusEffect> statusEffects);
}