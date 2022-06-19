using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

namespace PartyQuiz.Utils
{
    public static class EnumerableExtensions
    {
        public static T PickRandom<T>(this IEnumerable<T> array)
        {
            var enumerable = array as T[] ?? array.ToArray();

            var count = enumerable.Length;

            if (count <= 0)
                throw new Exception("Can't pick random from IEnumerable with size <= 0");

            if (count == 1)
                return enumerable.ElementAt(0);

            var index = Random.Range(0, count);
            return enumerable.ElementAt(index);
        }

        [CanBeNull]
        public static T PickRandom<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            var elements = enumerable.Where(predicate).ToArray();

            if (elements.Length <= 0)
                return default;

            if (elements.Length == 1)
                return elements.ElementAt(0);

            var index = Random.Range(0, elements.Length);

            return elements.ElementAt(index);
        }
    }
}