using PartyQuiz.Gameplay.Audio;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio.Board
{
    internal sealed class DisplayQuestionPanel : ObjectWithDisposableContainer
    {
        [SerializeField] private TextQuestionPanel _textQuestionPanel;
        [SerializeField] private PictureQuestionPanel _pictureQuestionPanel;
        [SerializeField] private AudioQuestionPanel _audioQuestionPanel;

        internal bool CanProceed => _textQuestionPanel.CanProceed
                                    && _pictureQuestionPanel.CanProceed
                                    && _audioQuestionPanel.CanProceed;

        internal void Init(AudioController audioController)
        {
            _textQuestionPanel.Init(audioController);
            _audioQuestionPanel.Init(audioController);
        }

        internal void Show(Question question)
        {
            _textQuestionPanel.Dispose();
            _pictureQuestionPanel.Dispose();
            _audioQuestionPanel.Dispose();

            SetPaused(false);
            ShowGameObject();

            if (!string.IsNullOrEmpty(question.Text.Value))
                _textQuestionPanel.Show(question.Text.Value);

            if (question.Picture.Value != null)
                _pictureQuestionPanel.Show(question.Picture.Value.Source);

            if (question.Audio.Value != null)
                _audioQuestionPanel.Show(question.Audio.Value.Source);
        }

        internal void SetPaused(bool isPaused)
        {
            _textQuestionPanel.SetPaused(isPaused);
            _pictureQuestionPanel.SetPaused(isPaused);
            _audioQuestionPanel.SetPaused(isPaused);
        }
    }
}