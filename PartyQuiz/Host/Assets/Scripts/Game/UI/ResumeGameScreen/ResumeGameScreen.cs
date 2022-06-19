using System;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PartyQuiz.Gameplay.UI
{
    internal sealed class ResumeGameScreen : UIElement
    {
        [SerializeField] private TMP_InputField _idInputField;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _backButton;
        
        private event Action<string> _onResumeButtonPressed;
        private event Action _onBackButtonPressed;

        internal void Init(Action<string> onResumeButtonPressed, Action onBackButtonPressed)
        {
            _onResumeButtonPressed = onResumeButtonPressed;
            _onBackButtonPressed = onBackButtonPressed;

            _resumeButton.onClick.AddListener(OnResumeButtonPressedHandler);
            DC.AddDisposable(() => _resumeButton.onClick.RemoveListener(OnResumeButtonPressedHandler));

            _backButton.onClick.AddListener(OnBackButtonPressedHandler);
            DC.AddDisposable(() => _backButton.onClick.RemoveListener(OnBackButtonPressedHandler));
        }

        private void OnResumeButtonPressedHandler()
        {
            _onResumeButtonPressed?.Invoke(_idInputField.text);
        }

        private void OnBackButtonPressedHandler()
        {
            _onBackButtonPressed?.Invoke();
        }
    }
}