using System.Collections.Generic;
using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Serialization.Importers;
using PartyQuiz.Structure.Runtime;
using UnityEngine;

namespace PartyQuiz.Serialization.Version2
{
    public struct ThemeV2 : IRuntimeCreation<Theme>
    {
        private const int MAX_QUESTIONS_COUNT = 5;

        public string Name;
        public List<QuestionV2> Questions;

        public Theme CreateRuntime(IPackImporter importer)
        {
            var questionCount = Mathf.Min(Questions.Count, MAX_QUESTIONS_COUNT);
            var questionsRuntime = new List<Question>(questionCount);

            for (var i = 0; i < questionCount; i++)
            {
                var question = Questions[i];
                questionsRuntime.Add(question.CreateRuntime(importer));
            }

            return new Theme(Name, questionsRuntime);
        }

        internal static ThemeV2 Export(Theme theme, IPackExporter exporter)
        {
            var questionCount = Mathf.Min(theme.Questions.Count, MAX_QUESTIONS_COUNT);
            var questions = new List<QuestionV2>(questionCount);

            for (var i = 0; i < questionCount; i++)
            {
                var questionPlan = theme.Questions[i];
                questions.Add(QuestionV2.Export(questionPlan, exporter));
            }

            return new ThemeV2
            {
                Name = theme.Name.Value,
                Questions = questions
            };
        }
    }
}