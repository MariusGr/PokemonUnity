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

public class BaseScriptableObject : ScriptableObject, IInstanceWithId
{
    [field: SerializeField]
    public string Id { get; private set; }
    private static Dictionary<string, BaseScriptableObject> instances = new Dictionary<string, BaseScriptableObject>();
    public static BaseScriptableObject Get(string id)
    {
        Debug.Log($"{id}");
        return instances[id];
    }

    public void AssignNewUID()
    {
        Id = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private void OnEnable()
    {
        if (this is null)
            Debug.LogError($"ID is still null on {GetType()}");

        while (instances.ContainsKey(Id))
        {
            AssignNewUID();
        }

        Debug.Log(Id);
        instances[Id] = this;
    }
}
