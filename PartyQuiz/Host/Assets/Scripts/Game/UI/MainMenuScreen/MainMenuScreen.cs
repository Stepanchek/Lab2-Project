using System;
using PartyQuiz.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PartyQuiz.Gameplay.UI
{
    internal sealed class MainMenuScreen : UIElement
    {
        [SerializeField] private Button _resumeGameButton;
        [SerializeField] private Button _newGameButton;

        private event Action _onResumeGameButtonPressed;
        private event Action _onNewGameButtonPressed;

        internal void Show(Action onResumeGameButtonPressed, Action onNewGameButtonPressed)
        {
            _onResumeGameButtonPressed = onResumeGameButtonPressed;
            _onNewGameButtonPressed = onNewGameButtonPressed;

            _resumeGameButton.onClick.AddListener(OnResumeGameButtonPressedHandler);
            DC.AddDisposable(() => _resumeGameButton.onClick.RemoveListener(OnResumeGameButtonPressedHandler));
            
            _newGameButton.onClick.AddListener(OnNewGameButtonPressedHandler);
            DC.AddDisposable(() => _newGameButton.onClick.RemoveListener(OnNewGameButtonPressedHandler));

            ShowGameObject();
        }

        private void OnResumeGameButtonPressedHandler()
        {
            _onResumeGameButtonPressed?.Invoke();
        }

        private void OnNewGameButtonPressedHandler()
        {
            _onNewGameButtonPressed?.Invoke();
        }
    }
}