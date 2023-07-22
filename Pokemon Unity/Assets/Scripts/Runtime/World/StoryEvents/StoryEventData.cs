using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJSON;

[SerializeField]
public abstract class StoryEventData
{
    abstract public IEnumerator Invoke();
}
