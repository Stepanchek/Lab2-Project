using System;
using UnityEngine;

namespace PartyQuiz.Utils
{
    public abstract class Poolable<TKey, TPool> : MonoBehaviour, IPoolable where TKey : Enum
    {
        [SerializeField] private TKey _key;

        public Enum Key => _key;
        
        protected TPool Pool { get; private set; }
        
        public void Init(object pool)
        {
            Pool = (TPool)pool;
        }
    }
}