using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJSON;

public abstract class StoryEvent : BaseScriptableObject, ISavable
{
    [field: SerializeField] public bool Happened { get; private set; }

    private void OnEnable()
    {
        SaveGameManager.Register(this);
        Initialize();
    }

    public void TryInvoke()
    {
        if (!Happened)
            Invoke();
    }

    virtual protected void Invoke() => Happened = true;
    public string GetKey() => Id;

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();
        json.Add("happened", Happened);
        return json;
    }

    public void LoadFromJSON(JSONObject json)
    {
        JSONNode jsonData = json[GetKey()];
        Happened = jsonData["happened"];
    }

    public void LoadDefault() => Happened = false;
}
