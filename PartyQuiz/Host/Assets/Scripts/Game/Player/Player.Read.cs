using System;
using Newtonsoft.Json;
using PartyQuiz.Gameplay.VFX;
using PartyQuiz.Network;
using UnityEngine;

namespace PartyQuiz.Gameplay.Players
{
    public sealed partial class Player
    {
        internal void ReadMessage(string input)
        {
            var message = JsonConvert.DeserializeObject<PlayerMessage>(input);
            
            if (message.SetRole != null)
                SetRole(message.SetRole);
            if (message.SetInfo != null)
                SetInfo(message.SetInfo);
            else if (message.ButtonPressed != null)
                ButtonPressed(message.ButtonPressed);
            else if (message.NextRound != null)
                NextRound(message.NextRound);
            else if (message.ResumeGame != null)
                ResumeGame(message.ResumeGame);
            else if (message.AnswerDecision != null)
                AnswerDecisionMade(message.AnswerDecision);
            else if (message.SelectedQuestion != null)
                QuestionSelected(message.SelectedQuestion);
            else if (message.CatInPokeOwnerSelected != null)
                SelectCatInPokeOwner(message.CatInPokeOwnerSelected);
            else if (message.AllHere != null)
                AllHere(message.AllHere);
            else if (message.PlayEmoji != null)
                PlayEmoji(message.PlayEmoji);
            else if (message.SkipSequence != null)
                SkipSequence(message.SkipSequence);
            else if (message.MakeABid != null)
                MakeABid(message.MakeABid);
            else if (message.Pass)
                AuctionPass();
            else if (message.EndAuction)
                EndAuction();
        }

        /// <summary>
        /// Init from already defined data (used for reconnection)
        /// </summary>
        internal void InitFromExisting(Player player)
        {
            _playerData = new PlayerData(player._playerData);
        }

        private void SetInfo(SetInfoResponse message)
        {
            var setInfoError = GetInfoError(message);

            if (setInfoError.Item1)
            {
                Name = message.Name;
                _gameController.AddAvatar(this, message.Avatar);
            }

            _messageWriter.SendObjectWithTargetPlayer(Id, new
            {
                Command = "InfoSetResult",
                IsInfoSet = setInfoError.Item1,
                Error = setInfoError.Item2,
            });
        }

        private (bool, string) GetInfoError(SetInfoResponse message)
        {
            if (string.IsNullOrEmpty(message.Name))
                return (false, "Name cannot be empty");
            
            if (string.IsNullOrEmpty(message.Avatar) || message.Avatar == "none")
                return (false, "Select your avatar first");

            return (true, string.Empty);
        }

        private void SetRole(string role)
        {
            _gameController.TrySetRole(this, role);
        }

        private void ButtonPressed(string message)
        {
            if (Role != ERole.Player)
                return;
            
            OnButtonPressed?.Invoke(this);
        }

        private void NextRound(string message)
        {
            _gameController.RequestNewRoundOrEndGame();
        }

        private void ResumeGame(string message)
        {
            _gameController.ResumeOrStartGame(this);
        }

        private void AnswerDecisionMade(string answerDecision)
        {
            var decision = answerDecision == "true";
            _gameController.NotifyAnswerDecision(decision);
        }

        private void QuestionSelected(SelectedQuestion question)
        {
            _gameController.NotifyQuestionSelected(question.ThemeId, question.QuestionId);
        }

        private void SelectCatInPokeOwner(string message)
        {
            _gameController.NotifyCatInPokeOwnerSelected(message);
        }

        private void AllHere(string message)
        {
            _gameController.NotifyAllHere();
        }

        public void PlayEmoji(string message)
        {
            var wasParsed = Enum.TryParse(message, out EPointParticleType particleType);

            if (wasParsed == false)
            {
                Debug.LogError($"Unknown emoji type: {message}");
                return;
            }

            OnEmojiPlayed?.Invoke(this, particleType);
        }

        private void SkipSequence(string message)
        {
            _gameController.SkipSequence();
        }

        private void MakeABid(MakeABid message)
        {
            _gameController.NotifyMakeABid(this, message.Bid, message.OldValue);
        }

        private void AuctionPass()
        {
            _gameController.NotifyAuctionPass(this);
        }

        private void EndAuction()
        {
            _gameController.NotifyEndAuction(this);
        }
    }
}
