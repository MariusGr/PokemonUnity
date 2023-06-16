using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public interface ISavable : IJSONConvertable
{
    public string GetKey();
    public void LoadFromJSON(JSONObject json);
    public void LoadDefault();
}
