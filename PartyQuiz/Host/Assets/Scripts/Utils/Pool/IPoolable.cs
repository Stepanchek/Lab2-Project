using System;

namespace PartyQuiz.Utils
{
    public interface IPoolable
    {
        Enum Key { get; }
        
        void Init(object pool);
    }
}