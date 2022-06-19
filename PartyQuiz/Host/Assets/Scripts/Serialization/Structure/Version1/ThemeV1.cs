using System.Collections.Generic;
using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Serialization.Importers;
using PartyQuiz.Structure.Runtime;
using UnityEngine;

namespace PartyQuiz.Serialization.Version1
{
    public struct ThemeV1 : IRuntimeCreation<Theme>
    {
        private const int MAX_QUESTIONS_COUNT = 5;

        public string Name;
        public List<QuestionV1> Questions;

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

        internal static ThemeV1 Export(Theme themePlan, IPackExporter exporter)
        {
            var questionCount = Mathf.Min(themePlan.Questions.Count, MAX_QUESTIONS_COUNT);
            var questions = new List<QuestionV1>(questionCount);

            for (var i = 0; i < questionCount; i++)
            {
                var questionPlan = themePlan.Questions[i];
                questions.Add(QuestionV1.Export(questionPlan, exporter));
            }

            return new ThemeV1
            {
                Name = themePlan.Name.Value,
                Questions = questions
            };
        }
    }
}