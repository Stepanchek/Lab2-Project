using System;
using JetBrains.Annotations;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Structure.Runtime;
using UnityEngine;

namespace PartyQuiz.Gameplay
{
    /// <summary>
    /// Questions related stuff, player started/stopped answering, question was either displayed or was hidden
    /// </summary>
    public sealed partial class GameController
    {
        internal event Action<Player> OnPlayerStartedAnswering;
        internal event Action<Player, bool> OnPlayerStoppedAnswering;
        
        internal event Action<Question, Theme> OnQuestionSelected;
        internal event Action<Question, Theme> OnQuestionDisplayed;

        internal event Action OnQuestionEnded;
        internal event Action<bool> OnAnswerDecisionMade;
        internal event Action OnGameOver;
        
        internal void DisplayQuestion(Question question, Theme theme)
        {
            switch (question.Type.Value)
            {
                case EQuestionType.CatInPoke:
                {
                    _audioController.PlayCatInPoke();
                    break;
                }
                case EQuestionType.Auction:
                {
                    _audioController.PlayAuction();
                    break;
                }
                case EQuestionType.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            OnQuestionDisplayed?.Invoke(question, theme);
        }

        public void NotifyAnswerDecision(bool value)
        {
            OnAnswerDecisionMade?.Invoke(value);
        }

        public void NotifyQuestionSelected(int themeId, int questionId)
        {
            var theme = CurrentRound.GetThemeByIndex(themeId);
            var question = theme.GetQuestionByIndex(questionId);

            foreach (var (id, _) in Players)
            {
                _messageWriter.SendObjectWithTargetPlayer(id, new
                {
                    Command = "QuestionSelected",
                    Theme = theme.Name.Value,
                    Price = question.Price.Value,
                    Answer = question.Answer.Text.Value,
                });
            }

            OnQuestionSelected?.Invoke(question, theme);
        }

        internal void NotifyPlayerStartedAnswering([CanBeNull] Player player, Question question)
        {
            if (player == null)
            {
                Debug.LogError("Can't NotifyPlayerStartedAnswering: Player is null");
                return;
            }

            OnPlayerStartedAnswering?.Invoke(player);

            foreach (var (id, existingPlayer) in Players)
            {
                var isHost = existingPlayer.Role == ERole.Host;

                _messageWriter.SendObjectWithTargetPlayer(id, new
                {
                    Command = "PlayerStartedAnswering",
                    IsYou = existingPlayer == player,
                    Answer = isHost ? question.Answer.Text.Value : string.Empty,
                    AnsweringPlayer = player.Name,
                });
            }
        }

        internal void NotifyPlayerStoppedAnswering([CanBeNull] Player player, bool wasCorrect)
        {
            OnPlayerStoppedAnswering?.Invoke(player, wasCorrect);

            if (player == null)
                return;

            foreach (var (id, existingPlayer) in Players)
            {
                _messageWriter.SendObjectWithTargetPlayer(id, new
                {
                    Command = "PlayerStoppedAnswering",
                    IsYou = existingPlayer == player,
                    TriedToAnswer = existingPlayer.TriedToAnswer,
                });
            }
        }

        internal void NotifyQuestionEnded()
        {
            foreach (var (_, player) in Players)
                player.TriedToAnswer = false;

            OnQuestionEnded?.Invoke();
        }

        internal void NotifyAllQuestionAnswered()
        {
            foreach (var (id, _) in Players)
            {
                _messageWriter.SendObjectWithTargetPlayer(id, new
                {
                    Command = "AllQuestionsAnswered",
                });
            }
        }
    }
}