using System.Collections.Generic;
using System.Linq;
using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Serialization.Importers;

namespace PartyQuiz.Serialization.Version1
{
    public struct PackV1 : IRuntimeCreation<Structure.Runtime.Pack>
    {
        public List<RoundV1> Rounds;

        public Structure.Runtime.Pack CreateRuntime(IPackImporter importer)
        {
            var roundsRuntime = Rounds.Select(round => round.CreateRuntime(importer)).ToList();

            return new Structure.Runtime.Pack(roundsRuntime);
        }

        internal static PackV1 Export(Structure.Runtime.Pack pack, IPackExporter exporter)
        {
            var rounds = pack.Rounds.Select(roundPlan => RoundV1.Export(roundPlan, exporter)).ToList();

            return new PackV1
            {
                Rounds = rounds
            };
        }
    }
}