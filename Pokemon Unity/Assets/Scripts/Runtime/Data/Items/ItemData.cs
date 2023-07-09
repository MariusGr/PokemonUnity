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

    public bool CanBeUsedOnOwnPokemon => category == ItemCategory.Medicine || category == ItemCategory.Food;
    public string Description => moveLearned is null ? description : moveLearned.description;
    public bool HealsHP => healsHPFully || hpHealed > 0;
    public bool HealsHPOnly => !HealsStatusEffectsNonVolatile && HealsHP;

    public bool HealsStatusEffectsNonVolatile => (healsAllStatusEffectsNonVolatile || !(nonVolatileStatusHealed is null));
    public bool HealsStatusEffectsNonVolatileOnly => !HealsStatusEffectsVolatile && HealsStatusEffectsNonVolatile && !HealsHP;
    public bool HealsStatusEffectNonVolatile(StatusEffect statusEffect)
        => !(statusEffect is null) && (healsAllStatusEffectsNonVolatile || nonVolatileStatusHealed == statusEffect.data);

    public bool HealsStatusEffectsVolatile => (healsAllStatusEffectsVolatile || !(volatileStatusHealed is null));
    public bool HealsStatusEffectsVolatileOnly => !healsAllStatusEffectsNonVolatile && HealsStatusEffectsVolatile && !HealsHP;
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
