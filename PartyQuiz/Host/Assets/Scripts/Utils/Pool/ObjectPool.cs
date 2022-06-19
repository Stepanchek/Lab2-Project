using System;
using System.Collections.Generic;

namespace PartyQuiz.Utils
{
    public abstract class ObjectPool<TValue> : IDisposable where TValue : IPoolable
    {
        private readonly Dictionary<Enum, List<TValue>> _pool = new();

        public TValue Spawn(TValue value)
        {
            var key = value.Key;
            
            if (_pool.ContainsKey(key) == false || _pool[key].Count <= 0)
                ForceCreate(value);

            var pooledValueList = _pool[key];
            var pooledValue = pooledValueList[0];

            pooledValueList.Remove(pooledValue);
            pooledValue.Init(this);

            return pooledValue;
        }

        /// <summary>
        /// Warning: Call the Despawn method from the particle!
        /// </summary>
        public void Despawn(TValue value)
        {
            var key = value.Key;

            if (_pool.ContainsKey(key) == false)
            {
                _pool.Add(key, new List<TValue> { value });

                return;
            }

            _pool[key].Add(value);
        }

        protected abstract void ForceCreate(TValue value);

        public void Dispose()
        {
            _pool.Clear();
        }
    }
}