using System;
using System.Collections.Generic;
using PartyQuiz.Utils;
using UniRx;

namespace PartyQuiz.Structure.Runtime
{
    public sealed class Theme
    {
        public readonly BindableCollection<Question> Questions;

        public readonly ReactiveProperty<string> Name = new("New Theme");

        public Theme(string name) : this(name, new List<Question>())
        {
        }

        internal Theme(string name, List<Question> questions)
        {
            Name.Value = name;
            Questions = new BindableCollection<Question>(questions);
        }

        public Question GetQuestionByIndex(int index)
        {
            if (Questions.Count <= 0)
                throw new Exception($"Questions count ({Questions.Count}) cannot be less than 0");

            if (index >= Questions.Count)
                throw new Exception($"Questions count ({Questions.Count}) is lower than requested ({index})");

            return Questions[index];
        }

        public int GetIndexByQuestion(Question question)
        {
            if (Questions.Contains(question) == false)
                throw new Exception($"Questions list does not contain {question.Text}");

            return Questions.IndexOf(question);
        }
    }
}