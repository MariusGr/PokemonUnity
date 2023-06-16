using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWaitingStatusEffect
{
    public IStatusEffectData Data { get; }
    //public int waitTimeRounds;

    //public WaitingStatusEffect(StatusEffectData statusEffect, int waitTimeRounds)
    //{
    //    data = statusEffect;
    //    this.waitTimeRounds = waitTimeRounds;
    //}
}
