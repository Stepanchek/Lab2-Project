using System.Collections.Generic;
using UnityEngine;

namespace PartyQuiz.Utils.Inputs
{
    public sealed class InputPreset
    {
        internal readonly Dictionary<KeyCode, ECommand> InputKeyCombinations = new();

        public InputPreset(params KeyValuePair<KeyCode, ECommand>[] combinations)
        {
            InputKeyCombinations.Clear();

            foreach (var combination in combinations)
                InputKeyCombinations.Add(combination.Key, combination.Value);
        }
    }
}