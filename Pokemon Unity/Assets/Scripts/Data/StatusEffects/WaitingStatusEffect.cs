using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingStatusEffect : IWaitingStatusEffect
{
    public IStatusEffectData Data { get; private set; }
    public int waitTimeRounds;

    public WaitingStatusEffect(IStatusEffectData statusEffect, int waitTimeRounds)
    {
        Data = statusEffect;
        this.waitTimeRounds = waitTimeRounds;
    }
}
