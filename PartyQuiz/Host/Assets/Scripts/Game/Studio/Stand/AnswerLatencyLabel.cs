using PartyQuiz.Gameplay.Players;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class AnswerLatencyLabel : ObjectWithDisposableContainer
    {
        [SerializeField] private GameObject _latencyView;
        [SerializeField] private TextMeshPro _latencyLabel;
        
        private Player _firstAnsweringPlayer;
        
        private float _latency;
        private bool _wasLatencyDisplayed;

        internal void Init(GameController gameController, Player player)
        {
            Reset();
            
            gameController.OnPlayerStartedAnswering += OnPlayerStartedAnsweringHandler;
            DC.AddDisposable(() => gameController.OnPlayerStartedAnswering -= OnPlayerStartedAnsweringHandler);

            gameController.OnPlayerStoppedAnswering += OnPlayerStoppedAnsweringHandler;
            DC.AddDisposable(() => gameController.OnPlayerStoppedAnswering -= OnPlayerStoppedAnsweringHandler);
            
            player.OnButtonPressed += OnPlayerButtonPressedHandler;
            DC.AddDisposable(() => player.OnButtonPressed -= OnPlayerButtonPressedHandler);
        }

        private void OnPlayerStartedAnsweringHandler(Player player)
        {
            if (_firstAnsweringPlayer != null)
                return;
            
            _firstAnsweringPlayer = player;
        }

        private void OnPlayerStoppedAnsweringHandler(Player player, bool wasCorrect)
        {
            Reset();
        }

        private void OnPlayerButtonPressedHandler(Player player)
        {
            if (player.TriedToAnswer)
                return;
            
            if (_wasLatencyDisplayed)
                return;

            if (_firstAnsweringPlayer == player)
                return;

            DisplayLatency();
        }
        
        private void DisplayLatency()
        {
            _wasLatencyDisplayed = true;
            
            var latency = _latency * 1000;
            var displayLatency = latency is > 0 and < 2000;

            _latencyView.SetActive(displayLatency);
            _latencyLabel.text = displayLatency ? latency.ToString("0") + "ms" : string.Empty;
        }

        private void Update()
        {
            if (_firstAnsweringPlayer == null)
                return;

            _latency += Time.deltaTime;
        }

        private void Reset()
        {
            _latencyView.SetActive(false);
            
            _latencyLabel.text = string.Empty;
            _firstAnsweringPlayer = null;

            _latency = 0f;
            _wasLatencyDisplayed = false;
        }
        
        public override void Dispose()
        {
            Reset();
            
            base.Dispose();
        }
    }
}