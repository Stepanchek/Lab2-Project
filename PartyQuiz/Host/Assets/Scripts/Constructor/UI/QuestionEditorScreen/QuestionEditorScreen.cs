using System;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PartyQuiz.Constructor
{
    internal sealed class QuestionEditorScreen : UIElement
    {
        [SerializeField] private TextMeshProUGUI _headerLabel;
        [SerializeField] private Button _closeButton;

        [SerializeField] private QuestionControlPanel _questionControlPanel;
        [SerializeField] private QuestionDataPanel _questionDataPanel;

        private Theme _theme;
        private Question _question;

        internal void Show(Theme theme, Question question, Action<Question> onDeleteButton)
        {
            _theme = theme;
            _question = question;

            _closeButton.OnClickAsObservable().SubscribeRx(_ => Close());
            
            _questionDataPanel.Show(question);
            
            _questionControlPanel.Show(question, Close, arg =>
            {
                onDeleteButton(arg);
                Dispose();
            });
            
            question.Price.SubscribeRx(_ => UpdateHeaderLabel());

            UpdateHeaderLabel();
            ShowGameObject();
        }

        private void UpdateHeaderLabel()
        {
            _headerLabel.text = $"{_theme.Name.Value}, {_question.Price.Value}";
        }

        internal void Close()
        {
            Dispose();
            
            _questionControlPanel.Dispose();
            _questionDataPanel.Dispose();
        }
    }
}