using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    public StatusEffectData data;
    public int lifeTime;
    public int stepCount;

    public StatusEffect(StatusEffectData data)
    {
        this.data = data;
        lifeTime = Random.Range(data.lifetimeRoundsMinimum, data.lifetimeRoundsMaximum);
        stepCount = 0;
    }
}
