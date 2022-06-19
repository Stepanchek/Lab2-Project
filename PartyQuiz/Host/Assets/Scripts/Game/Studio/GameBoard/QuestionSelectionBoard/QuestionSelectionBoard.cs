using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using Redcode.Awaiting;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio.Board
{
    internal sealed class QuestionSelectionBoard : ObjectWithDisposableContainer
    {
        private const float PAUSE_BEFORE_CAN_ANSWER = 1.5f;

        [SerializeField] private ThemePanel[] _themePanels;

        private GameController _gameController;

        private bool _selectionInProgress;

        internal void Display()
        {
            foreach (var themePanel in _themePanels)
                themePanel.Display();

            ShowGameObject();
        }

        internal void Init(GameController gameController)
        {
            _gameController = gameController;

            _gameController.OnQuestionSelected += OnQuestionSelectedHandler;
            DC.AddDisposable(() => _gameController.OnQuestionSelected -= OnQuestionSelectedHandler);
        }

        internal void SetRound(Round round)
        {
            foreach (var themePanel in _themePanels)
                themePanel.Dispose();

            for (var i = 0; i < round.Themes.Count; i++)
            {
                var theme = round.GetThemeByIndex(i);
                _themePanels[i].Show(theme);
            }

            Display();
        }

        private async void OnQuestionSelectedHandler(Question question, Theme theme)
        {
            if (_selectionInProgress)
                return;

            _selectionInProgress = true;

            foreach (var themePanel in _themePanels)
                themePanel.NotifyQuestionSelected(question, theme);

            await new WaitForSeconds(PAUSE_BEFORE_CAN_ANSWER);

            _gameController.DisplayQuestion(question, theme);

            _selectionInProgress = false;
        }
    }
}