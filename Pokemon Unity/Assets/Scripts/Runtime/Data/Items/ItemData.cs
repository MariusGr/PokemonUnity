using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item")]
public class ItemData : BaseScriptableObject
{
    public string fullName;
    public ItemCategory category;
    public float price;
    public Sprite icon;
    public string details;
    public string description;
    public MoveData moveLearned;
    public bool stacks;
    public bool consumable;
    public bool usableOnBattleOpponent;
    public bool catchesPokemon;
    public float catchRateBonus;
    public bool revives;
    public bool healsHPFully;
    public int hpHealed;
    public bool healsAllStatusEffectsNonVolatile;
    public bool healsAllStatusEffectsVolatile;
    public StatusEffectNonVolatileData nonVolatileStatusHealed;
    public StatusEffectVolatileData volatileStatusHealed;

    public bool canBeUsedOnOwnPokemon => category == ItemCategory.Medicine || category == ItemCategory.Food;
    public string Description => moveLearned is null ? description : moveLearned.description;
    public bool healsHP => healsHPFully || hpHealed > 0;
    public bool healsHPOnly => !healsStatusEffectsNonVolatile && healsHP;

    public bool healsStatusEffectsNonVolatile => (healsAllStatusEffectsNonVolatile || !(nonVolatileStatusHealed is null));
    public bool healsStatusEffectsNonVolatileOnly => !healsStatusEffectsVolatile && healsStatusEffectsNonVolatile && !healsHP;
    public bool HealsStatusEffectNonVolatile(StatusEffect statusEffect)
        => !(statusEffect is null) && (healsAllStatusEffectsNonVolatile || nonVolatileStatusHealed == statusEffect.data);

    public bool healsStatusEffectsVolatile => (healsAllStatusEffectsVolatile || !(volatileStatusHealed is null));
    public bool healsStatusEffectsVolatileOnly => !healsAllStatusEffectsNonVolatile && healsStatusEffectsVolatile && !healsHP;
    public bool HealsStatusEffectVolatile(List<StatusEffect> statusEffects)
    {
        if (healsAllStatusEffectsVolatile)
            return true;

        foreach(StatusEffect s in statusEffects)
            if (!(s is null) && volatileStatusHealed == s.data)
                return true;

        return false;
    }

    public override string ToString() => fullName;
}
