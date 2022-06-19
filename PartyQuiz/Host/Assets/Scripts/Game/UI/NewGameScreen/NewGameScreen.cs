using System;
using PartyQuiz.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PartyQuiz.Gameplay.UI
{
    internal sealed class NewGameScreen : UIElement
    {
        [SerializeField] private LoadPackPanel _loadPackPanel;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _backButton;
        
        private GameController _gameController;

        private event Action _onNewGameStarted;
        private event Action _onBackButtonPressed;

        internal void Init(GameController gameController, Action onNewGameStarted, Action onBackButtonPressed)
        {
            _gameController = gameController;
            
            _onNewGameStarted = onNewGameStarted;
            _onBackButtonPressed = onBackButtonPressed;
            
            _loadPackPanel.Show(OnPackLoadedHandler);
            
            _newGameButton.onClick.AddListener(OnNewGameButtonPressedHandler);
            DC.AddDisposable(() => _newGameButton.onClick.RemoveListener(OnNewGameButtonPressedHandler));
            
            _backButton.onClick.AddListener(OnBackButtonPressedHandler);
            DC.AddDisposable(() => _backButton.onClick.RemoveListener(OnBackButtonPressedHandler));
        }

        private void OnNewGameButtonPressedHandler()
        {
            _onNewGameStarted?.Invoke();
        }
        
        private void OnPackLoadedHandler(string path)
        {
            _gameController.LoadPq(path);
        }
        
        private void OnBackButtonPressedHandler()
        {
            _onBackButtonPressed?.Invoke();
        }
    }
}