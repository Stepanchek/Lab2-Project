using System.Linq;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class ThemePanel : ObjectWithDisposableContainer
    {
        [SerializeField] private BoardTextCell _themeNameCell;
        [SerializeField] private QuestionPanel[] _questionPanels;

        private Theme _theme;

        private bool AnyQuestionsToAnswer
        {
            get
            {
                if (_theme == null)
                    return false;

                return _theme.Questions.Count(x => !x.WasAnswered) > 0;
            }
        }

        internal void Display()
        {
            _themeNameCell.Roll(AnyQuestionsToAnswer, 0f);

            foreach (var questionPanel in _questionPanels)
            {
                var question = questionPanel.Question;

                if (question == null)
                    continue;

                questionPanel.Roll(question.WasAnswered == false, 0f);
            }
        }

        internal void Show(Theme theme)
        {
            _theme = theme;

            _themeNameCell.Show(theme.Name.Value);

            foreach (var questionPanel in _questionPanels)
                questionPanel.Dispose();

            for (var i = 0; i < theme.Questions.Count; i++)
            {
                var question = theme.GetQuestionByIndex(i);
                _questionPanels[i].Show(question);
            }

            ShowGameObject();
        }

        internal void NotifyQuestionSelected(Question question, Theme theme)
        {
            RollExcludeOtherQuestions(question, theme);
        }

        private void RollExcludeOtherQuestions(Question question, Theme theme)
        {
            if (AnyQuestionsToAnswer == false)
                return;

            var value = theme.Equals(_theme);

            if (value == false)
                _themeNameCell.Roll(false, 1.0f);

            foreach (var priceCell in _questionPanels)
            {
                var cellQuestion = priceCell.Question;

                if (cellQuestion == null || cellQuestion.WasAnswered)
                    continue;

                var isSameQuestion = question.Equals(cellQuestion);

                if (isSameQuestion == false)
                    priceCell.Roll(false, 0.5f);
            }
        }
    }
}