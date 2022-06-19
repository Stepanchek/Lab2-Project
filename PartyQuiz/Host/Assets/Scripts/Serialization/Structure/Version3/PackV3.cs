using System.Collections.Generic;
using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Serialization.Importers;
using PartyQuiz.Structure.Runtime;

namespace PartyQuiz.Serialization.Version3
{
    public struct PackV3 : IRuntimeCreation<Pack>
    {
        public List<RoundV3> Rounds;

        public Pack CreateRuntime(IPackImporter importer)
        {
            var roundsRuntime = new List<Round>(Rounds.Count);

            foreach (var round in Rounds)
                roundsRuntime.Add(round.CreateRuntime(importer));

            return new Pack(roundsRuntime);
        }

        internal static PackV3 Export(Pack pack, IPackExporter exporter)
        {
            var normalRounds = new List<RoundV3>(pack.Rounds.Count);
            var finalRounds = new List<RoundV3>(pack.Rounds.Count);

            foreach (var round in pack.Rounds)
            {
                var exported = RoundV3.Export(round, exporter);
             
                if (exported.Type == ERoundTypeV3.Normal)
                    normalRounds.Add(exported);
                else
                    finalRounds.Add(exported);
            }

            return new PackV3
            {
                Rounds = normalRounds,
            };
        }
    }
}