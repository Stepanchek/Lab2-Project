using System;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PartyQuiz.Constructor
{
    internal sealed class ThemeView : UIElement
    {
        [SerializeField] private Button _deleteButton;

        [SerializeField] private QuestionsListPanel _questionsListPanel;
        [SerializeField] private ThemeNamePanel _themeNamePanel;

        private Theme _theme;
        private Action<Theme> _onDeleteButtonPressed;

        internal void Show(Theme theme, Action<Theme> onDeleteButtonPressed)
        {
            DC.Dispose();
            
            _theme = theme;
            _onDeleteButtonPressed = onDeleteButtonPressed;

            _deleteButton.onClick.AddListener(OnDeleteButtonPressedHandler);
            DC.AddDisposable(() => _deleteButton.onClick.RemoveListener(OnDeleteButtonPressedHandler));
            
            _questionsListPanel.Show(theme);
            _themeNamePanel.Show(theme);
            
            ShowGameObject();
        }

        private void OnDeleteButtonPressedHandler()
        {
            _onDeleteButtonPressed?.Invoke(_theme);
        }
    }
}