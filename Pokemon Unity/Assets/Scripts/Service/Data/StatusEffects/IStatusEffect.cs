using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public interface IStatusEffect
{
    public IStatusEffectData Data { get; }
    public int LifeTime { get; set; }

    public bool OverTimeDamageTick();
    public JSONNode ToJSON();
}
