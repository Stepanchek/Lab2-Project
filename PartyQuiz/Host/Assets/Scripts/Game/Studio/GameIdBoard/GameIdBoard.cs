using PartyQuiz.Utils;
using TMPro;
using UniRx;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class GameIdBoard : ObjectWithDisposableContainer
    {
        [SerializeField] private TextMeshPro _gameIdLabel;
        
        internal void Init(GameController gameController)
        {
            gameController.GameId.SubscribeRx(OnGameIdChangedHandler);
        }

        private void OnGameIdChangedHandler(string gameId)
        {
            _gameIdLabel.text = gameId;
        }
    }
}