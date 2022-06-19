using PartyQuiz.Gameplay.Audio;
using PartyQuiz.Utils;
using PartyQuiz.Utils.TextTyper;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio.Board
{
    internal sealed class TextQuestionPanel : ObjectWithDisposableContainer, IQuestionPanel<string>
    {
        [SerializeField] private TextTyper _textTyper;

        private TextReader _textReader;

        private bool _finishedPrinting;

        public bool CanProceed => _finishedPrinting && _textReader.HasFinished;

        internal void Init(AudioController audioController)
        {
            _textReader = audioController.TextReader;
        }

        public void Show(string value)
        {
            _finishedPrinting = false;

            _textTyper.Stop();

            ShowGameObject();

            _textTyper.Type(value).HandleExceptions();

            _textTyper.OnPrintCompleted += OnPrintCompletedHandler;
            DC.AddDisposable(() => _textTyper.OnPrintCompleted -= OnPrintCompletedHandler);

            _textReader.Read(value).HandleExceptions();
        }

        private void OnPrintCompletedHandler()
        {
            _finishedPrinting = true;
        }

        public void SetPaused(bool isPaused)
        {
        }

        public override void Dispose()
        {
            _textTyper.Stop();

            if (_textReader != null)
                _textReader.Stop();

            OnPrintCompletedHandler();

            base.Dispose();
        }
    }
}