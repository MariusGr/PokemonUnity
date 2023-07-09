using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

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

    public StatusEffect(JSONNode json)
    {
        data = (StatusEffectData)BaseScriptableObject.Get(json["data"]);
        lifeTime = json["lifeTime"];
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
        json.Add("data", data.Id);
        json.Add("lifeTime", lifeTime);
        json.Add("stepCount", stepCount);
        return json;
    }
}
