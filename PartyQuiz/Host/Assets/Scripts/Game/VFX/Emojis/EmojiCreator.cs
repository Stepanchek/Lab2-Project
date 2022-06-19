using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Utils;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Gameplay.VFX
{
    internal sealed class EmojiCreator : ObjectWithDisposableContainer, IInitializable
    {
        [SerializeField] private PointParticleTypeDictionary _emojiParticles = new();
        
        private GameController _gameController;
        private Dictionary<Player, EPointParticleType> _playingPlayers = new();

        internal PointParticlePool PointParticlePool { get; private set; }

        [Inject]
        public void Construct(GameController gameController, PointParticlePool pointParticlePool)
        {
            _gameController = gameController;
            PointParticlePool = pointParticlePool;
        }

        public void Initialize()
        {
            _gameController.Players.OnItemAdded += OnPlayerAddedHandler;
            DC.AddDisposable(() => _gameController.Players.OnItemAdded -= OnPlayerAddedHandler);
        }

        private void OnPlayerAddedHandler(KeyValuePair<string, Player> keyValuePair)
        {
            var player = keyValuePair.Value;

            player.OnEmojiPlayed += OnEmojiPlayedHandler;
            DC.AddDisposable(() => player.OnEmojiPlayed -= OnEmojiPlayedHandler);
        }

        private void OnEmojiPlayedHandler(Player player, EPointParticleType type)
        {
            Play(player, type).HandleExceptions();
        }

        private async UniTask Play(Player player, EPointParticleType type)
        {
            if (_emojiParticles.ContainsKey(type) == false)
            {
                Debug.LogError($"Emoji with type {type} has not been found");
                return;
            }

            if (_playingPlayers.ContainsKey(player))
                return;

            _playingPlayers.Add(player, type);
            player.Avatar.SetEmojiPlayedState(type, true);
            
            var emojiView = PointParticlePool.Spawn(_emojiParticles[type]);
            await emojiView.Animate(player.Avatar.EmojiPoint.position);

            _playingPlayers.Remove(player);
            player.Avatar.SetEmojiPlayedState(type, false);
        }
    }
}