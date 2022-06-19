using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PartyQuiz.Utils
{
    public sealed class BindableCollection<T> : IEnumerable<T>
    {
        public event Action<T> OnItemAdded;
        public event Action OnClear;
        public event Action<T> OnItemRemoved;

        private readonly List<T> _collection;

        public int Count => _collection.Count;

        public T this[int index] => _collection.ElementAt(index);

        public BindableCollection()
        {
            _collection = new List<T>();
        }

        public BindableCollection(List<T> collection)
        {
            _collection = collection;
        }

        public void Add(T newItem)
        {
            if (Contains(newItem))
                return;

            _collection.Add(newItem);
            OnItemAdded?.Invoke(newItem);
        }

        public int IndexOf(T item)
        {
            return _collection.IndexOf(item);
        }

        public bool Contains(T item)
        {
            return _collection.Contains(item);
        }

        public void Remove(T newItem)
        {
            if (Contains(newItem) == false)
                return;

            _collection.Remove(newItem);
            OnItemRemoved?.Invoke(newItem);
        }

        public void Clear()
        {
            _collection.Clear();
            OnClear?.Invoke();
        }

        public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}