using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace PartyQuiz.Utils
{
    public sealed class BindableDictionary<TK, TV> : IDictionary<TK, TV>
    {
        public event Action<KeyValuePair<TK, TV>> OnItemAdded;
        public event Action OnClear;
        public event Action<KeyValuePair<TK, TV>> OnItemRemoved;

        public readonly Dictionary<TK, TV> CachedNodes = new();

        private readonly Dictionary<TK, TV> _dictionary;

        public ICollection<TK> Keys => _dictionary.Keys;
        public ICollection<TV> Values => _dictionary.Values;

        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        public BindableDictionary()
        {
            _dictionary = new Dictionary<TK, TV>();
        }

        public TV this[TK key]
        {
            get => _dictionary[key];
            set
            {
                var raiseRemoveEvent = _dictionary.ContainsKey(key);

                _dictionary[key] = value;
                var kvp = this.SingleOrDefault(x => Compare(x.Key, key));

                if (raiseRemoveEvent)
                    OnItemRemoved?.Invoke(kvp);

                OnItemAdded?.Invoke(kvp);
            }
        }

        public void Add(TK key, [CanBeNull] TV value)
        {
            _dictionary.Add(key, value);

            var kvp = this.SingleOrDefault(x => Compare(x.Key, key));

            if (CachedNodes.ContainsKey(key) == false)
                CachedNodes.Add(key, value);

            OnItemAdded?.Invoke(kvp);
        }

        public bool Remove(TK key)
        {
            var kvp = this.SingleOrDefault(x => Compare(x.Key, key));

            OnItemRemoved?.Invoke(kvp);

            var removed = _dictionary.Remove(key);

            if (removed == false)
                return false;

            CachedNodes.Remove(key);

            return true;
        }

        private bool Compare<T>(T x, T y)
        {
            return EqualityComparer<T>.Default.Equals(x, y);
        }

        public bool TryGetValue(TK key, out TV value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public bool ContainsKey(TK key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(KeyValuePair<TK, TV> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TK, TV> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TK, TV>>) _dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TK, TV> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            OnClear?.Invoke();

            _dictionary.Clear();
            CachedNodes.Clear();
        }
    }
}