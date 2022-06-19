using System;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Debug
{
    [Serializable]
    internal sealed class ConsoleLogColorsDictionary : SerializedDictionary<EConsoleLogType, Color>
    {
    }
}