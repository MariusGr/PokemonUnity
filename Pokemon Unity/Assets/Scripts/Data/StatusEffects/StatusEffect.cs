using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class StatusEffect : IStatusEffect
{
    [field: SerializeField] public IStatusEffectData Data { get; private set; }
    [field: SerializeField] public int LifeTime { get; set; }
    private int stepCount;

    public StatusEffect(IStatusEffectData data)
    {
        Data = data;
        LifeTime = Random.Range(Data.LifetimeRoundsMinimum, Data.LifetimeRoundsMaximum);
        stepCount = 0;
    }

    public StatusEffect(JSONNode json)
    {
        Data = (StatusEffectData)BaseScriptableObject.Get(json["data"]);
        LifeTime = json["lifeTime"];
        stepCount = json["stepCount"];
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

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();
        json.Add("data", Data.Id);
        json.Add("lifeTime", LifeTime);
        json.Add("stepCount", stepCount);
        return json;
    }
}
