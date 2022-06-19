using System.Collections.Generic;
using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Serialization.Importers;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Serialization.Version2
{
    public struct RoundV2 : IRuntimeCreation<Round>
    {
        private const int MAX_THEMES_COUNT = 5;

        public List<ThemeV2> Themes;

        public Round CreateRuntime(IPackImporter importer)
        {
            var themesCount = Mathf.Min(Themes.Count, MAX_THEMES_COUNT);
            var themesRuntime = new List<Theme>(themesCount);

            for (var i = 0; i < themesCount; i++)
            {
                var theme = Themes[i];
                themesRuntime.Add(theme.CreateRuntime(importer));
            }

            return new Round
            {
                Themes = new BindableCollection<Theme>(themesRuntime),
            };
        }

        internal static RoundV2 Export(Round round, IPackExporter exporter)
        {
            var themesCount = Mathf.Min(round.Themes.Count, MAX_THEMES_COUNT);
            var themes = new List<ThemeV2>(themesCount);

            for (var i = 0; i < themesCount; i++)
            {
                var themePlan = round.Themes[i];
                themes.Add(ThemeV2.Export(themePlan, exporter));
            }

            return new RoundV2
            {
                Themes = themes
            };
        }
    }
}