using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartyQuiz.Utils
{
    [Serializable]
    public abstract class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> _keys = new();
        [SerializeField] private List<TValue> _values = new();

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var (key, value) in this)
            {
                _keys.Add(key);
                _values.Add(value);
            }
        }

        public void OnAfterDeserialize()
        {
            for (var i = 0; i < _keys.Count; i++)
            {
                var key = _keys[i];

                if (ContainsKey(key))
                    continue;

                Add(key, _values[i]);
            }

            _keys.Clear();
            _values.Clear();
        }
    }
}