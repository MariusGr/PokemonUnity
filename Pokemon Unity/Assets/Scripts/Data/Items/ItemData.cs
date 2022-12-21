using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item")]
public class ItemData : ScriptableObject
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
    public bool healsAllStati;
    public StatusEffectNonVolatileData statusHealed;

    public bool canBeUsedOnOwnPokemon => category == ItemCategory.Medicine;
    public string Description => moveLearned is null ? description : moveLearned.description;
    public bool healsHP => healsHPFully || hpHealed > 0;
    public bool healsHPOnly => !healsStatusEffects && healsHP;
    public bool healsStatusEffects => (healsAllStati || !(statusHealed is null));
    public bool healsStatusEffectsOnly => healsStatusEffects && !healsHP;
    public bool healsStatusEffect(StatusEffectNonVolatileData statusEffect) => !(statusEffect is null) && (healsAllStati || statusHealed == statusEffect);
    public override string ToString() => fullName;
}
