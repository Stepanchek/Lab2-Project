using System;
using System.Collections.Generic;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PartyQuiz.Constructor
{
    internal sealed class QuestionControlPanel : UIElement
    {
        [SerializeField] private TMP_InputField _priceInputField;
        [SerializeField] private TMP_Dropdown _questionTypeDropdown;

        [SerializeField] private Button _okButton;
        [SerializeField] private Button _deleteButton;

        private Question _question;

        internal void Show(Question question, Action onOkButton, Action<Question> onDeleteButton)
        {
            _question = question;
            _priceInputField.text = question.Price.Value.ToString();

            _priceInputField.onEndEdit.AddListener(OnPriceEndEditHandler);
            DC.AddDisposable(() => _priceInputField.onEndEdit.RemoveListener(OnPriceEndEditHandler));

            var okButtonClick = _okButton.OnClickAsObservable().SubscribeRx(_ => onOkButton());
            DC.AddDisposable(okButtonClick);

            var deleteButtonClick = _deleteButton.OnClickAsObservable().SubscribeRx(_ => onDeleteButton(question));
            DC.AddDisposable(deleteButtonClick);
            
            AddDropdownOptions(question);
            DC.AddDisposable(() => _questionTypeDropdown.ClearOptions());
            
            _questionTypeDropdown.onValueChanged.AddListener(OnDropdownValueChangedHandler);
            DC.AddDisposable(() => _questionTypeDropdown.onValueChanged.RemoveListener(OnDropdownValueChangedHandler));

            ShowGameObject();
        }

        private void AddDropdownOptions(Question question)
        {
            var options = new List<string>();

            var values = (EQuestionType[])Enum.GetValues(typeof(EQuestionType));
            var initValue = 0;
            
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];

                if (value == question.Type.Value)
                    initValue = i;

                options.Add(value.ToString());
            }

            _questionTypeDropdown.AddOptions(options);
            _questionTypeDropdown.value = initValue;
        }

        private void OnDropdownValueChangedHandler(int index)
        {
            var value = _questionTypeDropdown.options[index];
            
            if (Enum.TryParse(value.text, out EQuestionType questionType) == false)
                return;

            _question.Type.Value = questionType;
        }

        private void OnPriceEndEditHandler(string value)
        {
            if (int.TryParse(value, out var price) == false)
                return;

            _question.Price.Value = price;
        }
    }
}