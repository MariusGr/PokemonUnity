using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectIdAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
public class ScriptableObjectIdDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        if (string.IsNullOrEmpty(property.stringValue))
        {
            property.stringValue = Guid.NewGuid().ToString();
            Debug.Log(Guid.NewGuid().ToString());
        }
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif

public class BaseScriptableObject : ScriptableObject
{
    [ScriptableObjectId]
    public string Id;
    private static Dictionary<string, BaseScriptableObject> instances = new Dictionary<string, BaseScriptableObject>();
    public static BaseScriptableObject Get(string id) => instances[id];

    private void OnValidate()
    {
        instances[Id] = this;
    }
}
