using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace PartyQuiz.Utils.TextTyper
{
    public sealed class TextTyper : MonoBehaviour
    {
        private const float DEFAULT_PRINT_DELAY = 0.02f;

        [SerializeField] private TMP_Text _label;

        private readonly List<char> _punctuationCharacters = new() { '.', ',', '!', '?' };

        internal event Action<string> OnCharacterPrinted;

        private string _printingText;
        private float _defaultPrintDelay;
        private Coroutine _typeTextCoroutine;

        public event Action OnPrintCompleted;

        private bool _isCompleted;

        /// <summary>
        /// Types the text into the Text component character by character, using the specified (optional) print delay per character.
        /// </summary>
        /// <param name="text">Text to type.</param>
        /// <param name="printDelay">Print delay (in seconds) per character.</param>
        public async UniTask Type(string text, float printDelay = 0.05f)
        {
            Stop();

            _defaultPrintDelay = printDelay > 0 ? printDelay : DEFAULT_PRINT_DELAY;
            _printingText = text;

            _typeTextCoroutine = StartCoroutine(TypeTextCharByChar(text));

            _isCompleted = false;

            await new WaitUntil(() => _isCompleted);
        }

        /// <summary>
        /// Skips the typing to the end.
        /// </summary>
        public void Skip()
        {
            Stop();

            var generator = new TypedTextGenerator();
            var typedText = generator.GetCompletedText(_printingText);

            _label.text = typedText.TextToPrint;
            OnTypewritingComplete();
        }

        public void Stop()
        {
            if (_typeTextCoroutine == null)
                return;

            StopCoroutine(_typeTextCoroutine);
            _typeTextCoroutine = null;
        }

        private IEnumerator TypeTextCharByChar(string text)
        {
            _label.text = string.Empty;

            var generator = new TypedTextGenerator();
            TypedTextGenerator.TypedText typedText;

            var printedCharCount = 0;

            do
            {
                typedText = generator.GetTypedTextAt(text, printedCharCount);
                _label.text = typedText.TextToPrint;

                OnCharacterPrinted?.Invoke(typedText.LastPrintedChar.ToString());

                ++printedCharCount;

                var delay = typedText.Delay > 0
                    ? typedText.Delay
                    : GetPrintDelayForCharacter(typedText.LastPrintedChar);

                yield return new WaitForSeconds(delay);
            } while (!typedText.IsComplete);

            _typeTextCoroutine = null;
            OnTypewritingComplete();
        }

        private float GetPrintDelayForCharacter(char characterToPrint)
        {
            // Then get the default print delay for the current character
            var punctuationDelay = _defaultPrintDelay * 8.0f;
            return _punctuationCharacters.Contains(characterToPrint) ? punctuationDelay : _defaultPrintDelay;
        }

        private void OnTypewritingComplete()
        {
            _isCompleted = true;

            OnPrintCompleted?.Invoke();
        }

        public void Clear()
        {
            Stop();

            _label.text = string.Empty;
        }
    }
}