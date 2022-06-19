using System.Collections.Generic;
using JetBrains.Annotations;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Gameplay.VFX;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class PlayersPlatform : ObjectWithDisposableContainer
    {
        [SerializeField] private Stand _standTemplate;
        [SerializeField] private PlayerPointsPool _playerPointsPool;
        
        [SerializeField] private PointParticle _smokeParticle;

        private GameController _gameController;
        private EmojiCreator _emojiCreator;

        internal void Init(GameController gameController, EmojiCreator emojiCreator)
        {
            _gameController = gameController;
            _emojiCreator = emojiCreator;

            _gameController.Players.OnItemAdded += OnPlayerAddedHandler;
            DC.AddDisposable(() => _gameController.Players.OnItemAdded -= OnPlayerAddedHandler);

            _gameController.Players.OnItemRemoved += OnPlayerRemovedHandler;
            DC.AddDisposable(() => _gameController.Players.OnItemRemoved -= OnPlayerRemovedHandler);

            _gameController.OnPlayerStartedAnswering += OnPlayerStartedAnsweringHandler;
            DC.AddDisposable(() => _gameController.OnPlayerStartedAnswering -= OnPlayerStartedAnsweringHandler);

            _gameController.OnPlayerStoppedAnswering += OnPlayerStoppedAnsweringHandler;
            DC.AddDisposable(() => _gameController.OnPlayerStoppedAnswering -= OnPlayerStoppedAnsweringHandler);
        }

        private void OnPlayerAddedHandler(KeyValuePair<string, Player> keyValuePair)
        {
            var player = keyValuePair.Value;
            var stand = Instantiate(_standTemplate);

            stand.Init(_gameController, player);
            player.SetStand(stand);

            player.OnRoleChanged += OnRoleChangedHandler;
            DC.AddDisposable(() => player.OnRoleChanged -= OnRoleChangedHandler);

            OnRoleChangedHandler(player);
        }

        private void OnRoleChangedHandler(Player player)
        {
            var stand = player.Stand;
            
            var oldStandPoint = stand.OriginalPoint;
            var newStandPoint = _playerPointsPool.Request(player);

            if (oldStandPoint != null)
                _playerPointsPool.ReturnToAvailable(oldStandPoint, player.Role);

            var setAsOriginal = player.Role != ERole.NotSet;
            
            stand.SetPoint(newStandPoint, setAsOriginal);
            
            if (setAsOriginal)
                PlaySmokeVfx(stand);
        }
        
        private void PlaySmokeVfx(Stand stand)
        {
            var vfx = _emojiCreator.PointParticlePool.Spawn(_smokeParticle);
            vfx.Animate(stand.transform.position).HandleExceptions();
        }

        private void OnPlayerRemovedHandler(KeyValuePair<string, Player> keyValuePair)
        {
            var stand = keyValuePair.Value.Stand;
            _playerPointsPool.SetToDefault(stand);
        }

        private static void OnPlayerStartedAnsweringHandler(Player player)
        {
            player.Stand.Move(new Vector3(0, 0, 1));
        }

        private static void OnPlayerStoppedAnsweringHandler([CanBeNull] Player player, bool wasCorrect)
        {
            player?.Stand.ResetToOriginal();
        }

        internal void DespawnEveryPlayer()
        {
            foreach (var (_, player) in _gameController.Players)
            {
                if (player.Role != ERole.Player)
                    continue;

                var stand = player.Stand;
                
                PlaySmokeVfx(stand);
                _playerPointsPool.SetToDefault(stand);
            }
        }
    }
}