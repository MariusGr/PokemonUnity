using System.Collections.Generic;
using UnityEngine;
using System;

// https://answers.unity.com/questions/460727/how-to-serialize-dictionary-with-unity-serializati.html

namespace CollectionExtensions
{
    [Serializable]
    public abstract class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        public List<TKey> keys = new List<TKey>();

        [SerializeField]
        public List<TValue> values = new List<TValue>();

        // save the dictionary to lists
        abstract public void OnBeforeSerialize();

        // load dictionary from lists
        virtual public void OnAfterDeserialize()
        {
            this.Clear();

            for (int i = 0; i < keys.Count; i++)
                this.Add(keys[i], values[i]);
        }
    }
}
