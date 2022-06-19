using System;
using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Audio;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using Redcode.Awaiting;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio.Board
{
    internal sealed class RevealAnswerBoard : ObjectWithDisposableContainer
    {
        [SerializeField] private RevealAnswerTextPanel _textPanel;
        [SerializeField] private RevealAnswerPicturePanel _picturePanel;

        private Action _onAnswerShown;

        internal async UniTask Show(Answer answer, TextReader textReader, Action onAnswerShown)
        {
            _onAnswerShown = onAnswerShown;

            _textPanel.Dispose();
            _picturePanel.Dispose();

            if (answer.IsValid())
            {
                ShowGameObject();

                if (!string.IsNullOrEmpty(answer.Text.Value))
                    await _textPanel.Show(textReader, answer.Text.Value);

                if (answer.Picture.Value != null)
                    await _picturePanel.Show(answer.Picture.Value.Source);

                await new WaitForSeconds(2.0f);
            }

            _onAnswerShown?.Invoke();
        }
    }
}