using System;
using PartyQuiz.Gameplay.Audio;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio.Board
{
    internal sealed class GameBoard : ObjectWithDisposableContainer
    {
        [SerializeField] private GameInfoBoard _gameInfoBoard;
        [SerializeField] private QuestionSelectionBoard _questionSelectionBoard;
        [SerializeField] private QuestionSolveBoard _questionSolveBoard;
        [SerializeField] private RevealAnswerBoard _revealAnswerBoard;
        [SerializeField] private StartNextRoundBoard _startNextRoundBoard;

        private GameController _gameController;
        private AudioController _audioController;

        private Question _catInPokeQuestion;
        private Theme _catInPokeTheme;

        private Question _auctionQuestion;
        private Theme _auctionTheme;

        private void Awake()
        {
            _gameInfoBoard.Dispose();
            _questionSelectionBoard.Dispose();
            _questionSolveBoard.Dispose();
            _revealAnswerBoard.Dispose();
            _startNextRoundBoard.Dispose();
        }

        internal void Init(GameController gameController, AudioController audioController)
        {
            _gameController = gameController;
            _audioController = audioController;

            _gameInfoBoard.Init(_gameController);
            _questionSelectionBoard.Init(_gameController);

            _gameController.OnRoundChanged += OnRoundChangedHandler;
            DC.AddDisposable(() => _gameController.OnRoundChanged -= OnRoundChangedHandler);

            _gameController.OnCatInPokeOwnerSelected += OnCatInPokeOwnerSelectedHandler;
            DC.AddDisposable(() => _gameController.OnCatInPokeOwnerSelected -= OnCatInPokeOwnerSelectedHandler);

            _gameController.OnQuestionDisplayed += OnQuestionDisplayedHandler;
            DC.AddDisposable(() => _gameController.OnQuestionDisplayed -= OnQuestionDisplayedHandler);

            _questionSolveBoard.OnQuestionEnded += OnQuestionEndedHandler;
            DC.AddDisposable(() => _questionSolveBoard.OnQuestionEnded -= OnQuestionEndedHandler);

            _gameController.OnAuctionFinished += OnAuctionFinishedHandler;
            DC.AddDisposable(() => _gameController.OnAuctionFinished -= OnAuctionFinishedHandler);

            _gameInfoBoard.Display();
            _questionSelectionBoard.HideGameObject();
        }

        private void OnRoundChangedHandler(Round round)
        {
            if (round.Type == ERoundType.Final)
                return;

            _audioController.PlayNextRoundSfx();

            _startNextRoundBoard.HideGameObject();
            _gameInfoBoard.Dispose();
            _questionSelectionBoard.SetRound(round);
        }

        private void OnQuestionDisplayedHandler(Question question, Theme theme)
        {
            _questionSelectionBoard.HideGameObject();

            switch (question.Type.Value)
            {
                case EQuestionType.Normal:
                    DisplayQuestion(question, question.Price.Value, theme, null);
                    break;
                case EQuestionType.CatInPoke:
                    ShowCatInPoke(question, theme);
                    break;
                case EQuestionType.Auction:
                    ShowAuction(question, theme);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowCatInPoke(Question question, Theme theme)
        {
            _catInPokeQuestion = question;
            _catInPokeTheme = theme;

            _gameController.NotifyCatInPoke(_catInPokeQuestion, _catInPokeTheme);
        }

        private void ShowAuction(Question question, Theme theme)
        {
            _auctionQuestion = question;
            _auctionTheme = theme;

            _gameController.NotifyAuction(_auctionQuestion, _auctionTheme);
        }

        private void OnCatInPokeOwnerSelectedHandler(Player player)
        {
            DisplayQuestion(_catInPokeQuestion, _catInPokeQuestion.Price.Value, _catInPokeTheme, player);
        }

        private void OnAuctionFinishedHandler(Player highestBidder, int highestBid)
        {
            DisplayQuestion(_auctionQuestion, highestBid, _auctionTheme, highestBidder);
        }

        private void DisplayQuestion(Question question, int questionPrice, Theme theme, Player answeringPlayer)
        {
            var index = theme.GetIndexByQuestion(question);

            _questionSolveBoard.Init(_gameController, _audioController);
            _questionSolveBoard.Show(question, questionPrice, index, answeringPlayer).HandleExceptions();
        }

        private void OnQuestionEndedHandler(Question question, bool isCorrect)
        {
            _revealAnswerBoard.Show(question.Answer, _audioController.TextReader, OnAnswerShownHandler).HandleExceptions();

            _questionSolveBoard.Close();
        }

        private void OnAnswerShownHandler()
        {
            _gameController.NotifyQuestionEnded();

            if (_gameController.AreAllQuestionsAnswered)
                TryStartNextRound();
            else
                ContinueWithQuestions();

            _revealAnswerBoard.Dispose();
        }

        private void TryStartNextRound()
        {
            _gameController.NotifyAllQuestionAnswered();

            _questionSelectionBoard.HideGameObject();
            _startNextRoundBoard.Show(_gameController.CurrentRoundIndex);
        }

        private void ContinueWithQuestions()
        {
            _questionSelectionBoard.Display();
            _gameController.ContinueGame();
        }
    }
}