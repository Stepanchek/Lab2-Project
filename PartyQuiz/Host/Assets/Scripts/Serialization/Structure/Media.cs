using UnityEngine;

namespace PartyQuiz.Structure.Runtime
{
    public sealed class Media<T> where T : Object
    {
        public readonly T Source;
        public readonly string Path;

        public Media(T source, string path)
        {
            Source = source;
            Path = path;
        }
    }
}