using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingStatusEffect
{
    public StatusEffectData data;
    public int waitTimeRounds;

    public WaitingStatusEffect(StatusEffectData statusEffect, int waitTimeRounds)
    {
        data = statusEffect;
        this.waitTimeRounds = waitTimeRounds;
    }
}
