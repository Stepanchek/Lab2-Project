using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UniRx;
using UnityEngine;

namespace PartyQuiz.Constructor
{
    internal sealed class QuestionsListPanel : UIElement
    {
        [SerializeField] private QuestionView _questionViewTemplate;
        [SerializeField] private RectTransform _container;

        [SerializeField] private NewListView _newQuestionView;
        [SerializeField] private QuestionEditorScreen _questionEditorScreen;
        
        [SerializeField] private MessageWindow _confirmationWindow;
        [SerializeField] private QuestionTooltip _questionTooltip;

        private Theme _theme;

        internal void Show(Theme theme)
        {
            _theme = theme;

            DC.Dispose();
            
            var viewList = new BindableViewList<Question, QuestionView>(
                _theme.Questions,
                _questionViewTemplate,
                _container,
                (item, view) =>
                {
                    view.Show(item, isOver => OnQuestionHoverHandler(item, view, isOver), OnQuestionPressedHandler);
                    
                    _newQuestionView.SetAsLast();
                    ValidateAddQuestionButton();
                });

            DC.AddDisposable(viewList);
            
            _theme.Questions.OnItemRemoved += OnItemRemovedHandler;
            DC.AddDisposable(() => _theme.Questions.OnItemRemoved -= OnItemRemovedHandler);

            _newQuestionView.Show(OnNewQuestionCreatedHandler);
            _newQuestionView.SetAsLast();
            
            ValidateAddQuestionButton();
            ShowGameObject();
        }

        private void OnQuestionHoverHandler(Question question, QuestionView view, bool isOver)
        {
            if (isOver)
                _questionTooltip.Show(view, question);
            else
                _questionTooltip.Dispose();
        }

        private void OnQuestionPressedHandler(Question question)
        {
            _questionEditorScreen.Show(_theme, question, OnQuestionDeletedHandler);
        }

        private void OnQuestionDeletedHandler(Question question)
        {
            _confirmationWindow.Show(null, () => _theme.Questions.Remove(question));
        }

        private void AddQuestion(Question question)
        {
            _theme.Questions.Add(question);
        }

        private void OnNewQuestionCreatedHandler()
        {
            var price = _theme.Questions.Count <= 0 ? 100 : _theme.Questions[^1].Price.Value + 100;

            var question = new Question
            {
                Price = new ReactiveProperty<int>(price),
            };

            AddQuestion(question);
        }

        private void OnItemRemovedHandler(Question question)
        {
            ValidateAddQuestionButton();
        }

        private void ValidateAddQuestionButton()
        {
            if (_theme.Questions.Count >= Serialization.Version3.ThemeV3.MAX_QUESTIONS_COUNT)
                _newQuestionView.HideGameObject();
            else
                _newQuestionView.ShowGameObject();
        }
    }
}