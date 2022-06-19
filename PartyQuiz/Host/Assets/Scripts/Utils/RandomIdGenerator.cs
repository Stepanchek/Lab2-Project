using System.Collections.Generic;

namespace PartyQuiz.Utils
{
    public static class RandomIdGenerator
    {
        private static readonly string[] _consonants =
        {
            "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z",
        };

        private static readonly string[] _vowels =
        {
            "a", "e", "i", "u",
        };

        private static readonly string[] _numbers =
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
        };

        public static string GenerateRandomId()
        {
            var word = string.Empty;

            const int lettersCount = 4;

            for (var i = 0; i < lettersCount; i += 2)
                word += GetRandomSign(_consonants) + GetRandomSign(_vowels);

            const int numbersCount = 2;

            for (var i = 0; i < numbersCount; i++)
                word += GetRandomSign(_numbers);

            return word.ToUpper();
        }

        private static string GetRandomSign(IReadOnlyList<string> letters)
        {
            return letters[UnityEngine.Random.Range(0, letters.Count - 1)];
        }
    }
}