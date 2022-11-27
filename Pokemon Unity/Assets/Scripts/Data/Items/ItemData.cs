using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item")]
public class ItemData : ScriptableObject
{
    public string fullName;
    public ItemCategory category;
    public Sprite icon;
    public string details;
    public string description;
    public MoveData moveLearned;
    public bool stacks;
    public bool healsHPFully;
    public int hpHealed;
    public Status statusHealed;

    public bool canBeUsedOnOwnPokemon => category == ItemCategory.Medicine;

    public override string ToString()
    {
        return fullName;
    }
}
