using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    public StatusEffectData data;
    public int lifeTime;
    private int stepCount;

    public StatusEffect(StatusEffectData data)
    {
        this.data = data;
        lifeTime = Random.Range(data.lifetimeRoundsMinimum, data.lifetimeRoundsMaximum);
        stepCount = 0;
    }

    public bool OverTimeDamageTick()
    {
        stepCount += 1;
        if (stepCount >= 4)
        {
            stepCount = 0;
            return true;
        }

        return false;
    }
}
