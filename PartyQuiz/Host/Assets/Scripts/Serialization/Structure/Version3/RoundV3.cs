using System;
using System.Collections.Generic;
using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Serialization.Importers;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Serialization.Version3
{
    public enum ERoundTypeV3
    {
        Normal = 0,
        Final = 1,
    }

    public struct RoundV3 : IRuntimeCreation<Round>
    {
        private const int MAX_THEMES_COUNT = 5;

        public ERoundTypeV3 Type;
        public List<ThemeV3> Themes;

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
                Type = Type switch
                {
                    ERoundTypeV3.Normal => ERoundType.Normal,
                    ERoundTypeV3.Final => ERoundType.Final,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Themes = new BindableCollection<Theme>(themesRuntime),
            };
        }

        internal static RoundV3 Export(Round round, IPackExporter exporter)
        {
            var themesCount = Mathf.Min(round.Themes.Count, MAX_THEMES_COUNT);
            var themes = new List<ThemeV3>(themesCount);

            for (var i = 0; i < themesCount; i++)
            {
                var themePlan = round.Themes[i];
                themes.Add(ThemeV3.Export(themePlan, exporter));
            }

            return new RoundV3
            {
                Type = round.Type switch
                {
                    ERoundType.Normal => ERoundTypeV3.Normal,
                    ERoundType.Final => ERoundTypeV3.Final,
                    _ => throw new ArgumentOutOfRangeException(nameof(round), round, null)
                },
                Themes = themes,
            };
        }
    }
}