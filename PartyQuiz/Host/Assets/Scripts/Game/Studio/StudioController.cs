using PartyQuiz.Gameplay.Audio;
using PartyQuiz.Gameplay.Studio.Board;
using PartyQuiz.Gameplay.VFX;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class StudioController : MonoBehaviour, IInitializable
    {
        [SerializeField] private GameBoard _gameBoard;
        [SerializeField] private PlayersPlatform _playersPlatform;
        [SerializeField] private RoundResultBoard _roundResultBoard;
        [SerializeField] private GameIdBoard _gameIdBoard;

        private GameController _gameController;
        private AudioController _audioController;
        private EmojiCreator _emojiCreator;

        [Inject]
        public void Construct(GameController gameController, AudioController audioController, EmojiCreator emojiCreator)
        {
            _gameController = gameController;
            _audioController = audioController;
            _emojiCreator = emojiCreator;
        }
        
        public void Initialize()
        {
            _gameBoard.Init(_gameController, _audioController);
            _playersPlatform.Init(_gameController, _emojiCreator);
            _roundResultBoard.Init(_gameController);
            _gameIdBoard.Init(_gameController);
        }
    }
}