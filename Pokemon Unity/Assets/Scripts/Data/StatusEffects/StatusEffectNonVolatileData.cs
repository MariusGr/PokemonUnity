using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNonVolatileStatusEffect", menuName = "Pokemon/NonVolatileStatusEffect")]
public class StatusEffectNonVolatileData : StatusEffectData, IStatusEffectNonVolatileData
{
    public Sprite icon;
}
