using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AYellowpaper;

namespace CollectionExtensions
{
    [Serializable]
    public class DictionaryWithInterfaceTypeKeys<TKeyInterface, TValue, TValueBase> : InspectorFriendlySerializableDictionary<TKeyInterface, TValue>
        where TValueBase : UnityEngine.Object
        where TKeyInterface : class
    {
        [SerializeField] List<InterfaceReference<TKeyInterface, TValueBase>> keysInterface = new List<InterfaceReference<TKeyInterface, TValueBase>>();

        override public void OnAfterDeserialize()
        {
            Clear();

            for (int i = 0; i < keys.Count; i++)
                Add(keysInterface[i], values[i]);
        }
    }
}
