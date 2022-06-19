using JetBrains.Annotations;
using PartyQuiz.Utils;
using TMPro;
using UniRx;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio.Board
{
    internal sealed class GameInfoBoard : ObjectWithDisposableContainer
    {
        [SerializeField] private TextMeshPro _gameIdLabel;

        internal void Display()
        {
            _gameIdLabel.text = string.Empty;

            ShowGameObject();
        }

        internal void Init(GameController gameController)
        {
            gameController.GameId.SubscribeRx(OnGameIdChangedHandler);
        }

        private void OnGameIdChangedHandler([CanBeNull] string gameId)
        {
            if (string.IsNullOrEmpty(gameId))
                return;

            _gameIdLabel.text = gameId;
        }
    }
}