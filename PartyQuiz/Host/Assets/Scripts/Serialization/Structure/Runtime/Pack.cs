using System;
using System.Collections.Generic;
using PartyQuiz.Serialization;
using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Utils;
using SFB;
using UniRx;
using UnityEngine;

namespace PartyQuiz.Structure.Runtime
{
    public sealed class Pack
    {
        public readonly BindableCollection<Round> Rounds = new();
        
        public readonly ReactiveProperty<string> Name = new("New Pack");

        public Pack()
        {
        }

        internal Pack(List<Round> rounds)
        {
            Rounds = new BindableCollection<Round>(rounds);
        }

        public Round GetRoundByIndex(int index)
        {
            if (Rounds.Count <= 0)
                throw new Exception($"Rounds count ({Rounds.Count}) cannot be less than 0");

            if (index >= Rounds.Count)
                throw new Exception($"Rounds count ({Rounds.Count}) is lower than requested ({index})");

            return Rounds[index];
        }

        public void AssignRandom(EQuestionType type, int count)
        {
            foreach (var round in Rounds)
                round.AssignRandom(type, count);
        }

        public void Export()
        {
            try
            {
                var packName = Name.Value;
                var packPath = StandaloneFileBrowser.SaveFilePanel("Save pack", null, packName, "pq");

                var exporter = new PackExporter(packPath);
                Helper.Export(this, exporter);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error exporting the pack: {e}");
            }
        }
    }
}