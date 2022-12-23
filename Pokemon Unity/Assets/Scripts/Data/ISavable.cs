using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public interface ISavable
{
    public string GetKey();
    public JSONObject ToJSON();
    public void LoadFromJSON(JSONObject json);
}
