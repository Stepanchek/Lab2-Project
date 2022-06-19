using System;
using JetBrains.Annotations;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Structure.Runtime
{
    public enum ERoundType
    {
        Normal = 0,
        Final = 1,
    }

    public sealed class Round
    {
        public ERoundType Type = ERoundType.Normal;
        public BindableCollection<Theme> Themes = new();

        public Theme GetThemeByIndex(int index)
        {
            if (Themes.Count <= 0)
                throw new Exception($"Themes count ({Themes.Count}) cannot be less than 0");

            if (index >= Themes.Count)
                throw new Exception($"Themes count ({Themes.Count}) is lower than requested ({index})");

            return Themes[index];
        }

        internal void AssignRandom(EQuestionType type, int count)
        {
            ResetQuestionTypes(type);

            for (var i = 0; i < count; i++)
                AssignRandom(type);
        }

        private void ResetQuestionTypes(EQuestionType type)
        {
            foreach (var theme in Themes)
            {
                foreach (var question in theme.Questions)
                {
                    if (question.Type.Value == type)
                        question.Type.Value = EQuestionType.Normal;
                }
            }
        }

        private void AssignRandom(EQuestionType type)
        {
            var question = GetRandomQuestion();

            if (question == null)
            {
                Debug.LogError($"Error assigning {type}: {GetHashCode()}");
                return;
            }

            question.Type.Value = type;

            Debug.Log($"{type}. Question price: {question.Price.Value}");
        }

        [CanBeNull]
        private Question GetRandomQuestion()
        {
            if (Themes.Count <= 0)
                return null;
            
            var theme = Themes.PickRandom();
            var question = theme?.Questions.PickRandom(x => x.Type.Value == EQuestionType.Normal);

            return question;
        }
    }
}