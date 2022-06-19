using System;
using System.Collections.Generic;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Structure.Runtime;
using UnityEngine;

namespace PartyQuiz.Gameplay
{
    /// <summary>
    /// Cat in poke handling
    /// </summary>
    public sealed partial class GameController
    {
        internal event Action<Question, Theme> OnCatInPokeCalled;
        internal event Action<Player> OnCatInPokeOwnerSelected;
        
        internal void NotifyCatInPoke(Question question, Theme theme)
        {
            OnCatInPokeCalled?.Invoke(question, theme);
        }

        internal void ConfirmCatInPoke()
        {
            var candidates = GetCatInPokeCandidates();

            foreach (var (id, _) in Players)
            {
                _messageWriter.SendObjectWithTargetPlayer(id, new
                {
                    Command = "StartCatInPoke",
                    Players = candidates,
                });
            }
        }
        
        private List<CatInPokeCandidate> GetCatInPokeCandidates()
        {
            var result = new List<CatInPokeCandidate>();

            foreach (var (_, player) in Players)
            {
                if (player.Role != ERole.Player)
                    continue;

                var candidate = new CatInPokeCandidate(player);
                result.Add(candidate);
            }

            return result;
        }
        
        internal void NotifyCatInPokeOwnerSelected(string playerId)
        {
            if (Players.ContainsKey(playerId) == false)
            {
                Debug.LogError($"Cannot set cat in poke owner {playerId}");
                return;
            }

            var player = Players[playerId];
            OnCatInPokeOwnerSelected?.Invoke(player);
        }
    }
}