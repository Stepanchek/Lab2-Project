using System;
using System.Linq;
using JetBrains.Annotations;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Gameplay
{
    /// <summary>
    /// Players participating, player pressed a button or was added/removed
    /// </summary>
    public sealed partial class GameController
    {
        private const int MAX_PLAYERS_COUNT = 7;

        internal event Action<Player> OnPlayerButtonPressed;

        public BindableDictionary<string, Player> Players { get; } = new();
        public Player Selector { get; private set; }

        internal Player[] PlayersOrderedByScore
        {
            get
            {
                return Players.Values
                    .Where(x => x.Role == ERole.Player)
                    .OrderByDescending(x => x.Score)
                    .ToArray();
            }
        }
        
        internal Player LowestScorePlayer
        {
            get
            {
                var orderedSequence = Players.OrderBy(x => x.Value.Score);
                var allEligiblePlayers = orderedSequence.Where(x => x.Value.Role == ERole.Player);

                return allEligiblePlayers.First().Value;
            }
        }

        public void AddPlayer(Player player)
        {
            var id = player.Id;

            if (Players.ContainsKey(id))
            {
                var connectedPlayer = Players[id];
                player.InitFromExisting(connectedPlayer);

                Players[id] = player;
            }
            else
            {
                Players.Add(id, player);
            }

            player.OnButtonPressed += OnPlayerButtonPressedHandler;
        }

        internal void AddAvatar(Player player, string avatarName)
        {
            if (player.Avatar != null)
            {
                Debug.LogError($"Player {player.Name} already has an avatar");
                return;
            }
            
            var avatar = _avatarFactory.Request(player, avatarName);
            player.SetAvatar(avatar);
        }

        internal void RemovePlayer(string id)
        {
            if (Players.ContainsKey(id) == false)
            {
                Debug.LogError($"Cannot remove player with id {id}");
                return;
            }

            var player = Players[id];
            player.OnButtonPressed -= OnPlayerButtonPressedHandler;

            Players.Remove(id);
        }

        private void OnPlayerButtonPressedHandler(Player player)
        {
            OnPlayerButtonPressed?.Invoke(player);
        }

        internal void TrySetRole(Player player, string roleText)
        {
            var role = roleText == "host" ? ERole.Host : ERole.Player;

            var canSetRole = CanSetRole(role);

            if (canSetRole.Item1)
                player.Role = role;

            _messageWriter.SendObjectWithTargetPlayer(player.Id, new
            {
                Command = "SetRoleResult",
                IsRoleSet = canSetRole.Item1,
                Error = canSetRole.Item2,
                Role = roleText,
            });
        }

        private (bool, string) CanSetRole(ERole role)
        {
            switch (role)
            {
                case ERole.Player:
                {
                    var canSet = Players.Count(x => x.Value.Role == ERole.Player) < MAX_PLAYERS_COUNT;
                    var error = canSet ? string.Empty : "Too many players";

                    return (canSet, error);
                }
                case ERole.Host:
                {
                    var canSet = Players.Any(x => x.Value.Role == ERole.Host) == false;
                    var error = canSet ? string.Empty : "There is already a host";

                    return (canSet, error);
                }
                case ERole.NotSet:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal void PlayerGotMessage(string id, string message)
        {
            if (Players.ContainsKey(id) == false)
            {
                Debug.LogError($"Cannot transfer message to the player with id {id}");
                return;
            }

            var player = Players[id];
            player.ReadMessage(message);
        }

        internal void SetPlayerAsSelector([CanBeNull] Player selector)
        {
            if (selector == null)
            {
                Debug.LogError("Selector cannot be null");
                return;
            }
            
            foreach (var (_, player) in Players)
                player.IsSelector = false;

            selector.IsSelector = true;

            Selector = selector;
        }

        internal void NotifyCanAnswer()
        {
            OnCanAnswer?.Invoke();
            
            foreach (var (id, _) in Players)
            {
                _messageWriter.SendObjectWithTargetPlayer(id, new
                {
                    Command = "CanAnswer",
                });
            }
        }
    }
}