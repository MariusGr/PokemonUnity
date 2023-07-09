using System.Collections.Generic;
using UnityEngine;
using System;

namespace CollectionExtensions
{
    [Serializable]
    public class ToolFriendlySerializableDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue>
    {
        // save the dictionary to lists
        override public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        override public void OnAfterDeserialize()
        {
            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            base.OnAfterDeserialize();
        }
    }
}
