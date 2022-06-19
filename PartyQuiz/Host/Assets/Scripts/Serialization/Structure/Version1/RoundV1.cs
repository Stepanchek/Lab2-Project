using System.Collections.Generic;
using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Serialization.Importers;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Serialization.Version1
{
    public struct RoundV1 : IRuntimeCreation<Structure.Runtime.Round>
    {
        private const int MAX_THEMES_COUNT = 5;

        public List<ThemeV1> Themes;

        public Structure.Runtime.Round CreateRuntime(IPackImporter importer)
        {
            var themesCount = Mathf.Min(Themes.Count, MAX_THEMES_COUNT);
            var themesRuntime = new List<Structure.Runtime.Theme>(themesCount);

            for (var i = 0; i < themesCount; i++)
            {
                var theme = Themes[i];
                themesRuntime.Add(theme.CreateRuntime(importer));
            }

            return new Structure.Runtime.Round
            {
                Themes = new BindableCollection<Structure.Runtime.Theme>(themesRuntime),
            };
        }

        internal static RoundV1 Export(Structure.Runtime.Round round, IPackExporter exporter)
        {
            var themesCount = Mathf.Min(round.Themes.Count, MAX_THEMES_COUNT);
            var themes = new List<ThemeV1>(themesCount);

            for (var i = 0; i < themesCount; i++)
            {
                var themePlan = round.Themes[i];
                themes.Add(ThemeV1.Export(themePlan, exporter));
            }

            return new RoundV1
            {
                Themes = themes
            };
        }
    }
}