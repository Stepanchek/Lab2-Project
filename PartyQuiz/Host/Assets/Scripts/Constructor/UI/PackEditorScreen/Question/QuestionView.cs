using System;
using JetBrains.Annotations;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PartyQuiz.Constructor
{
    internal sealed class QuestionView : UIElement
    {
        [SerializeField] private Button _questionButton;
        [SerializeField] private Image _backgroundImage;
        
        [SerializeField] private Color _goodColor;
        [SerializeField] private Color _badColor;
        
        [SerializeField] private TextMeshProUGUI _priceLabel;
        [SerializeField] private Image _icon;

        [SerializeField] private Sprite _catIcon;
        [SerializeField] private Sprite _auctionIcon;

        private Question _question;
        private Action<bool> _onHover;

        internal void Show(Question question, Action<bool> onHover, Action<Question> onPressedHandler)
        {
            _question = question;
            _onHover = onHover;

            _questionButton.OnClickAsObservable().SubscribeRx(_ => onPressedHandler(question));
            
            question.Price.SubscribeToText(_priceLabel);
            question.Type.SubscribeRx(_ => ValidateIcon());
            question.Text.SubscribeRx(_ => ValidateBackground());
            question.Picture.SubscribeRx(_ => ValidateBackground());
            question.Audio.SubscribeRx(_ => ValidateBackground());
            
            question.Answer.Text.SubscribeRx(_ => ValidateBackground());
            question.Answer.Picture.SubscribeRx(_ => ValidateBackground());
            
            ValidateIcon();
            ValidateBackground();
            
            ShowGameObject();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            _onHover?.Invoke(true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            _onHover?.Invoke(false);
        }

        private void ValidateBackground()
        {
            var isValid = _question.IsValid() && _question.Answer.IsValid();
            _backgroundImage.color = isValid ? _goodColor : _badColor;
        }

        private void ValidateIcon()
        {
            var icon = GetQuestionIcon(_question);

            _icon.sprite = icon;
            _icon.gameObject.SetActive(icon != null);
        }

        [CanBeNull]
        private Sprite GetQuestionIcon(Question question)
        {
            return question.Type.Value switch
            {
                EQuestionType.Normal => null,
                EQuestionType.CatInPoke => _catIcon,
                EQuestionType.Auction => _auctionIcon,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}