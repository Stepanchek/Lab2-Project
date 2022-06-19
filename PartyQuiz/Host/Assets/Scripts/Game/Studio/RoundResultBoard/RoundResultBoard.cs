using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Gameplay.Sequences;
using PartyQuiz.Utils;
using Redcode.Awaiting;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class RoundResultBoard : ObjectWithDisposableContainer, ISequencePlayer
    {
        private const float ROLL_SPEED = 1.0f;

        [SerializeField] private PlayerResultPanel[] _playerResultPanels;

        private GameController _gameController;

        private void Awake()
        {
            RollAll(false, 0f);
        }

        internal void Init(GameController gameController)
        {
            _gameController = gameController;

            ShowBoardForCurrentPlayers();

            gameController.Players.OnItemAdded += OnPlayerAddedHandler;
            DC.AddDisposable(() => gameController.Players.OnItemAdded -= OnPlayerAddedHandler);

            gameController.Players.OnItemRemoved += OnItemRemovedHandler;
            DC.AddDisposable(() => gameController.Players.OnItemRemoved -= OnItemRemovedHandler);
            
            gameController.OnNewRoundRequested += OnNewRoundRequestedHandler;
            DC.AddDisposable(() => gameController.OnNewRoundRequested -= OnNewRoundRequestedHandler);

            ShowGameObject();
        }

        private void OnPlayerAddedHandler(KeyValuePair<string, Player> keyValuePair)
        {
            var player = keyValuePair.Value;

            player.OnRoleChanged += OnRoleChangedHandler;
            DC.AddDisposable(() => player.OnRoleChanged -= OnRoleChangedHandler);

            OnRoleChangedHandler(player);

            player.OnScoreChanged += OnScoreChangedHandler;
            DC.AddDisposable(() => player.OnScoreChanged -= OnScoreChangedHandler);
        }

        private void OnScoreChangedHandler(Player player, int diff)
        {
            var orderedPlayers = _gameController.PlayersOrderedByScore;

            for (var i = 0; i < _playerResultPanels.Length; i++)
            {
                if (i >= _playerResultPanels.Length)
                    continue;
                
                var view = _playerResultPanels[i];

                if (i >= orderedPlayers.Length)
                    continue;
                
                var playerChanged = view.Player != orderedPlayers[i];
                var shouldRoll = view.Player == player || playerChanged;

                if (shouldRoll)
                    DisplayScore(view, orderedPlayers[i]).HandleExceptions();
            }
        }
        
        private static async UniTask DisplayScore(PlayerResultPanel view, Player player)
        {
            await view.Roll(false, ROLL_SPEED);

            view.Show(player);
            
            await view.Roll(true, ROLL_SPEED);
        }

        private void OnItemRemovedHandler(KeyValuePair<string, Player> keyValuePair)
        {
            var player = keyValuePair.Value;
            var view = ViewByPlayer(keyValuePair.Value);

            if (view == null)
            {
                Debug.LogError($"Panel for player {player.Name} has not been found!");
                return;
            }

            view.Dispose();
        }

        private void OnRoleChangedHandler(Player player)
        {
            if (player.Role == ERole.Player)
                AddPlayerToResults(player);
        }

        private void AddPlayerToResults(Player player)
        {
            if (player.Role != ERole.Player)
                return;

            var emptyPanel = ViewByPlayer(null);

            if (emptyPanel == null)
            {
                Debug.LogError("No more empty panels!");
                return;
            }

            emptyPanel.Roll(true, ROLL_SPEED).HandleExceptions();
            emptyPanel.Show(player);
        }

        private void OnNewRoundRequestedHandler()
        {
            RollAll(false, ROLL_SPEED);
        }

        private void ShowBoardForCurrentPlayers()
        {
            var allPlayers = _gameController.Players.Values.Where(x => x.Role == ERole.Player).ToArray();

            for (var i = 0; i < allPlayers.Length; i++)
            {
                var player = allPlayers[i];
                _playerResultPanels[i].Show(player);
            }
        }

        public async UniTask Play(Sequence sequence)
        {
            var player = (Player)sequence.Key;
            var view = ViewByPlayer(player);

            if (view == null)
                return;
            
            view.Roll(true, ROLL_SPEED).HandleExceptions();

            await new WaitForSeconds(2.0f);
        }

        public void Stop()
        {
        }

        private void RollAll(bool value, float time)
        {
            foreach (var playerResultPanel in _playerResultPanels)
                playerResultPanel.Roll(value, time).HandleExceptions();
        }

        [CanBeNull]
        private PlayerResultPanel ViewByPlayer(Player player)
        {
            var view = _playerResultPanels.FirstOrDefault(x => x.Player == player);

            if (view == null)
            {
                Debug.LogError($"Cannot find PlayerResultView for {player.Name}");
                return null;
            }

            return view;
        }
    }
}