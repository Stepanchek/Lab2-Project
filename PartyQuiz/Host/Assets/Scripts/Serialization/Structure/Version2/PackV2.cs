using System.Collections.Generic;
using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Serialization.Importers;
using PartyQuiz.Structure.Runtime;

namespace PartyQuiz.Serialization.Version2
{
    public struct PackV2 : IRuntimeCreation<Pack>
    {
        public List<RoundV2> Rounds;

        public Pack CreateRuntime(IPackImporter importer)
        {
            var roundsRuntime = new List<Round>(Rounds.Count);

            foreach (var round in Rounds)
                roundsRuntime.Add(round.CreateRuntime(importer));

            return new Pack(roundsRuntime);
        }

        internal static PackV2 Export(Pack pack, IPackExporter exporter)
        {
            var rounds = new List<RoundV2>(pack.Rounds.Count);

            foreach (var roundPlan in pack.Rounds)
                rounds.Add(RoundV2.Export(roundPlan, exporter));

            return new PackV2
            {
                Rounds = rounds
            };
        }
    }
}