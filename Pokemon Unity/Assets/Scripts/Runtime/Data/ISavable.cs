using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public interface ISavable
{
    public string GetKey();
    public JSONNode ToJSON();
    public void LoadFromJSON(JSONNode json);
    public void LoadDefault();
}
