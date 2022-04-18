using System.Collections.Generic;
using UnityEngine;
using System;

namespace CollectionExtensions
{
    [Serializable]
    public class InspectorFriendlySerializableDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue>
    {
        override public void OnBeforeSerialize() { }

        // load dictionary from lists
        override public void OnAfterDeserialize()
        {
            if (keys.Count == values.Count)
                base.OnAfterDeserialize();
        }
    }
}
