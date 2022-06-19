using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Network;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using Redcode.Awaiting;
using UnityEngine;

namespace PartyQuiz.Gameplay
{
    /// <summary>
    /// Game connection, player connected/disconnected, round changed
    /// </summary>
    public sealed partial class GameController
    {
        internal event Action OnNewRoundRequested;
        internal event Action<Round> OnRoundChanged;

        internal void NotifyPlayerConnected(string id)
        {
            if (Players.ContainsKey(id) == false)
            {
                Debug.LogError($"Cannot set player with id {id} as connected");
                return;
            }

            var player = Players[id];
            player.IsOnline = true;
        }

        internal void NotifyPlayerDisconnected(string id)
        {
            if (Players.ContainsKey(id) == false)
            {
                Debug.LogError($"Cannot set player with id {id} as disconnected");
                return;
            }

            var player = Players[id];
            player.IsOnline = false;
        }

        internal async void TryToReconnect()
        {
            while (_socketServer.WebSocket.State != WebSocketState.Open)
            {
                await new WaitForSeconds(1);

                if (string.IsNullOrEmpty(GameId.Value))
                {
                    GameId.Value = RandomIdGenerator.GenerateRandomId();
                    continue;
                }

                _socketServer.Run(GameId.Value).HandleExceptions();
                await new WaitForSeconds(0.5f);
            }
        }

        public void RequestNewRoundOrEndGame()
        {
            if (CurrentRoundIndex + 1 >= MaxRoundsCount)
                NotifyGameEnded();
            else
                RequestNextRound();
        }

        private void NotifyGameEnded()
        {
            OnGameOver?.Invoke();

            _socketServer.Stop(GameId.Value).HandleExceptions();
        }

        /// <summary>
        /// Preliminary request the next round
        /// </summary>
        private void RequestNextRound()
        {
            OnNewRoundRequested?.Invoke();
        }

        /// <summary>
        /// Forcefully switch to the next round, notifying every board
        /// </summary>
        internal void NextRound(bool updateSelector)
        {
            if (Players.Any(x => x.Value.Role == ERole.Player) == false)
                return;

            if (updateSelector)
                SetPlayerAsSelector(LowestScorePlayer);

            CurrentRoundIndex++;

            CurrentRound = _pack.GetRoundByIndex(CurrentRoundIndex);
            OnRoundChanged?.Invoke(CurrentRound);

            foreach (var (_, existingPlayer) in Players)
                ResumeOrStartGame(existingPlayer);
        }

        /// <summary>
        /// Resume from disconnection or start a new game
        /// </summary>
        internal void ResumeOrStartGame(Player player)
        {
            var id = player.Id;

            switch (player.Role)
            {
                case ERole.Player:
                {
                    _messageWriter.SendObjectWithTargetPlayer(id, new
                    {
                        Command = "StartGame",
                        player.Name,
                        player.Score
                    });

                    break;
                }
                case ERole.Host:
                {
                    var result = new List<HostTheme>();
                    var themes = CurrentRound.Themes;

                    for (var i = 0; i < themes.Count; i++)
                    {
                        var theme = themes[i];
                        var hostQuestions = new List<HostQuestion>();

                        for (var j = 0; j < theme.Questions.Count; j++)
                        {
                            var question = theme.Questions[j];
                            var hostQuestion = new HostQuestion(j, question.Price.Value, question.WasAnswered);

                            hostQuestions.Add(hostQuestion);
                        }

                        var hostTheme = new HostTheme(i, theme.Name.Value, hostQuestions);
                        result.Add(hostTheme);
                    }

                    _messageWriter.SendObjectWithTargetPlayer(id, new
                    {
                        Command = "StartGame",
                        Themes = result,
                    });

                    break;
                }
                case ERole.NotSet:
                default:
                {
                    Debug.LogError("Unknown role" + player.Role);
                    return;
                }
            }
        }
    }
}