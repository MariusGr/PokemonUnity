using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AYellowpaper;

namespace CollectionExtensions
{
    [Serializable]
    public class DictionaryWithInterfaceTypeValues<TKey, TValueInterface, TValueBase> : InspectorFriendlySerializableDictionary<TKey, TValueInterface>
        where TValueBase : UnityEngine.Object
        where TValueInterface : class
    {
        [SerializeField] List<InterfaceReference<TValueInterface, TValueBase>> valuesInterface = new List<InterfaceReference<TValueInterface, TValueBase>>();

        override public void OnAfterDeserialize()
        {
            Clear();

            for (int i = 0; i < keys.Count; i++)
                Add(keys[i], valuesInterface[i]);
        }
    }
}
