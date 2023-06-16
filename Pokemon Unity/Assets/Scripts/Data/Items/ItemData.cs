using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item")]
public class ItemData : BaseScriptableObject, IItemData
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public ItemCategory Category { get; private set; }
    [field: SerializeField] public float Price { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Details { get; private set; }
    [field: SerializeField] public IMoveData MoveLearned { get; private set; }
    [field: SerializeField] public bool Stacks { get; private set; }
    [field: SerializeField] public bool Consumable { get; private set; }
    [field: SerializeField] public bool UsableOnBattleOpponent { get; private set; }
    [field: SerializeField] public bool CatchesPokemon { get; private set; }
    [field: SerializeField] public float CatchRateBonus { get; private set; }
    [field: SerializeField] public bool Revives { get; private set; }
    [field: SerializeField] public bool HealsHPFully { get; private set; }
    [field: SerializeField] public int HpHealed { get; private set; }
    [field: SerializeField] public bool HealsAllStatusEffectsNonVolatile { get; private set; }
    [field: SerializeField] public bool HealsAllStatusEffectsVolatile { get; private set; }
    [field: SerializeField] public IStatusEffectNonVolatileData NonVolatileStatusHealed { get; private set; }
    [field: SerializeField] public IStatusEffectVolatileData VolatileStatusHealed { get; private set; }

    public bool CanBeUsedOnOwnPokemon => Category == ItemCategory.Medicine || Category == ItemCategory.Food;
    [SerializeField] private string description;
    public string Description => MoveLearned is null ? description : MoveLearned.Description;
    public bool HealsHP => HealsHPFully || HpHealed > 0;
    public bool HealsHPOnly => !HealsStatusEffectsNonVolatile && HealsHP;

    public bool HealsStatusEffectsNonVolatile => (HealsAllStatusEffectsNonVolatile || !(NonVolatileStatusHealed is null));
    public bool HealsStatusEffectsNonVolatileOnly => !HealsStatusEffectsVolatile && HealsStatusEffectsNonVolatile && !HealsHP;
    public bool HealsStatusEffectNonVolatile(IStatusEffect statusEffect)
        => !(statusEffect is null) && (HealsAllStatusEffectsNonVolatile || NonVolatileStatusHealed == statusEffect.Data);

    public bool HealsStatusEffectsVolatile => (HealsAllStatusEffectsVolatile || !(VolatileStatusHealed is null));
    public bool HealsStatusEffectsVolatileOnly => !HealsAllStatusEffectsNonVolatile && HealsStatusEffectsVolatile && !HealsHP;
    public bool HealsStatusEffectVolatile(List<IStatusEffect> statusEffects)
    {
        if (HealsAllStatusEffectsVolatile)
            return true;

        foreach (StatusEffect s in statusEffects)
            if (!(s is null) && VolatileStatusHealed == s.Data)
                return true;

        return false;
    }

    public override string ToString() => Name;
}
