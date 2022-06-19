using System;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PartyQuiz.Constructor
{
    internal sealed class RoundView : UIElement
    {
        [SerializeField] private TextMeshProUGUI _roundLabel;
        
        [SerializeField] private Image _background;
        [SerializeField] private Color _selectedColor;

        [SerializeField] private Button _editRoundButton;
        [SerializeField] private Button _deleteButton;

        private Round _round;
        private Color _defaultColor;
        
        private Action<Round> _onEditRoundButtonPressed;
        private Action<Round> _onDeleteButtonPressed;
        
        internal void Show(Round round, int index, Action<Round> onEditRoundButtonPressed,
            Action<Round> onDeleteButtonPressed)
        {
            _round = round;
            _defaultColor = _background.color;
            _onEditRoundButtonPressed = onEditRoundButtonPressed;
            _onDeleteButtonPressed = onDeleteButtonPressed;

            _roundLabel.text = $"Round {index + 1}";

            _editRoundButton.onClick.AddListener(OnEditRoundButtonPressedHandler);
            DC.AddDisposable(() => _editRoundButton.onClick.RemoveListener(OnEditRoundButtonPressedHandler));

            _deleteButton.onClick.AddListener(OnDeleteButtonPressedHandler);
            DC.AddDisposable(() => _deleteButton.onClick.RemoveListener(OnDeleteButtonPressedHandler));

            SetCanDeleteState(false);
            ShowGameObject();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            SetCanDeleteState(true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            SetCanDeleteState(false);
        }

        internal void SetSelectedState(bool isSelected)
        {
            _background.color = isSelected ? _selectedColor : _defaultColor;
        }

        private void SetCanDeleteState(bool value)
        {
            _deleteButton.gameObject.SetActive(value);
        }

        private void OnDeleteButtonPressedHandler()
        {
            _onDeleteButtonPressed?.Invoke(_round);
        }

        private void OnEditRoundButtonPressedHandler()
        {
            _onEditRoundButtonPressed?.Invoke(_round);
        }
    }
}