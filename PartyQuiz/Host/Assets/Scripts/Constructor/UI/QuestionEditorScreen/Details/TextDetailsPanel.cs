using System;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;

namespace PartyQuiz.Constructor
{
    internal sealed class TextDetailsPanel : UIElement
    {
        [SerializeField] private TMP_InputField _inputField;

        private Action<string> _onTextSet;

        internal void Show(string text, Action<string> onTextSet)
        {
            _onTextSet = onTextSet;

            _inputField.onEndEdit.AddListener(OnTextEndEditHandler);
            DC.AddDisposable(() => _inputField.onEndEdit.RemoveListener(OnTextEndEditHandler));

            _inputField.text = text;
            
            ShowGameObject();
        }

        private void OnTextEndEditHandler(string value)
        {
            _onTextSet?.Invoke(value);
        }
    }
}