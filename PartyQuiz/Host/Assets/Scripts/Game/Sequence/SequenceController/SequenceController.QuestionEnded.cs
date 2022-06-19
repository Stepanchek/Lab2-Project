using Cysharp.Threading.Tasks;
using PartyQuiz.Utils;

namespace PartyQuiz.Gameplay.Sequences
{
    public sealed partial class SequenceController
    {
        private void SubscribeQuestionEnded()
        {
            _gameController.OnQuestionEnded += OnQuestionEndedHandler;
            DC.AddDisposable(() => _gameController.OnQuestionEnded -= OnQuestionEndedHandler);
        }
        
        private void OnQuestionEndedHandler()
        {
            Play(DeclareSelector).HandleExceptions();
        }

        private async UniTask DeclareSelector()
        {
            if (_gameController.AreAllQuestionsAnswered)
                return;

            var selector = _gameController.Selector.Name;

            var sequence = new Sequence().SetText($"{selector}");
            Run(sequence, _textReader).HandleExceptions();
            
            var yourTurn = new Sequence().SetText($"{selector}, your turn!").SetCameraType(ECameraPointType.Overall).SetStopAfterDone(false);
            await Run(yourTurn, _hostSpeechBubble);
        }
    }
}