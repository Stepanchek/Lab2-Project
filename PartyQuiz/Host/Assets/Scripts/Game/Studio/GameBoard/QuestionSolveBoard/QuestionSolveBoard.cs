using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using PartyQuiz.Gameplay.Audio;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using Redcode.Awaiting;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio.Board
{
    internal sealed class QuestionSolveBoard : ObjectWithDisposableContainer
    {
        [SerializeField] private TimerPanel _solveTimerPanel;
        [SerializeField] private PlayerTimerPanel _playerTimerPanel;
        [SerializeField] private DisplayQuestionPanel _displayQuestionPanel;

        private GameController _gameController;
        private AudioController _audioController;

        private Question _question;
        private int _questionPrice;

        private int _questionIndex;

        private Player _guessingPlayer;
        private bool _canAcceptAnswers;

        /// <summary>
        /// The question has ended, either solved or the timer has finished
        /// </summary>
        internal event Action<Question, bool> OnQuestionEnded;

        internal void Init(GameController gameController, AudioController audioController)
        {
            _gameController = gameController;
            _audioController = audioController;

            _gameController.OnPlayerButtonPressed += OnPlayerButtonPressedHandler;
            DC.AddDisposable(() => _gameController.OnPlayerButtonPressed -= OnPlayerButtonPressedHandler);

            _gameController.OnAnswerDecisionMade += OnAnswerDecisionMadeHandler;
            DC.AddDisposable(() => _gameController.OnAnswerDecisionMade -= OnAnswerDecisionMadeHandler);

            _solveTimerPanel.Stop();

            _solveTimerPanel.OnTimerEnded += OnSolveTimerEndedHandler;
            DC.AddDisposable(() => _solveTimerPanel.OnTimerEnded -= OnSolveTimerEndedHandler);

            _playerTimerPanel.Stop();

            _playerTimerPanel.OnTimerEnded += OnPlayerTimerEndedHandler;
            DC.AddDisposable(() => _playerTimerPanel.OnTimerEnded -= OnPlayerTimerEndedHandler);

            _displayQuestionPanel.Init(_audioController);
        }

        internal async UniTask Show(Question question, int questionPrice, int index, [CanBeNull] Player answeringPlayer)
        {
            _question = question;
            _questionPrice = questionPrice;
            _questionIndex = index;

            _question.WasAnswered = true;

            // Show before the timer initialization because of the coroutines
            ShowGameObject();

            _displayQuestionPanel.Show(_question);
            _canAcceptAnswers = false;

            await new WaitUntil(() => _displayQuestionPanel.CanProceed);

            if (question.Type.Value == EQuestionType.Normal)
                _gameController.NotifyCanAnswer();
            
            _canAcceptAnswers = true;
            _solveTimerPanel.Run(15);

            if (answeringPlayer != null)
                HandleCatInPokeCarrier(answeringPlayer);
        }

        private void HandleCatInPokeCarrier(Player player)
        {
            _gameController.SetPlayerAsSelector(player);

            PlayerAnswers(player, 20);
        }

        private void OnPlayerButtonPressedHandler(Player player)
        {
            if (_canAcceptAnswers == false)
                return;

            if (player.TriedToAnswer)
                return;

            if (_guessingPlayer != null)
                return;

            PlayerAnswers(player, 10);

            _displayQuestionPanel.SetPaused(true);
        }

        /// <summary>
        /// Set player as answering
        /// </summary>
        private void PlayerAnswers(Player player, float answerTime)
        {
            _guessingPlayer = player;

            _gameController.NotifyPlayerStartedAnswering(_guessingPlayer, _question);

            _solveTimerPanel.Pause();

            _playerTimerPanel.SetAnsweringPlayer(player);
            _playerTimerPanel.Run(answerTime);

            player.TriedToAnswer = true;
        }

        private void OnAnswerDecisionMadeHandler(bool isCorrect)
        {
            if (isCorrect)
                PlayerCorrect();
            else
                PlayerIncorrect().HandleExceptions();

            _playerTimerPanel.Stop();
        }

        /// <summary>
        /// Player answers correctly
        /// </summary>
        private void PlayerCorrect()
        {
            _audioController.PlayCorrectAnswer(_questionIndex);

            _gameController.SetPlayerAsSelector(_guessingPlayer);
            _guessingPlayer.Score += _questionPrice;

            StopGuessing(true);
            _solveTimerPanel.Stop();

            OnQuestionEnded?.Invoke(_question, true);
        }

        /// <summary>
        /// Player answers incorrectly
        /// </summary>
        private async UniTask PlayerIncorrect()
        {
            _audioController.PlayIncorrectAnswer();

            _guessingPlayer.Score -= _questionPrice;

            StopGuessing(false);
            await new WaitForSeconds(1.5f);

            _solveTimerPanel.Unpause();
            _displayQuestionPanel.SetPaused(false);
        }

        private void OnSolveTimerEndedHandler()
        {
            StopGuessing(false);
            OnQuestionEnded?.Invoke(_question, false);
        }

        private void OnPlayerTimerEndedHandler()
        {
        }

        private void StopGuessing(bool wasCorrect)
        {
            _playerTimerPanel.Stop();

            _gameController.NotifyPlayerStoppedAnswering(_guessingPlayer, wasCorrect);
            _guessingPlayer = null;
        }

        public void Close()
        {
            _solveTimerPanel.Dispose();
            _playerTimerPanel.Dispose();
            _displayQuestionPanel.SetPaused(true);

            Dispose();
        }
    }
}