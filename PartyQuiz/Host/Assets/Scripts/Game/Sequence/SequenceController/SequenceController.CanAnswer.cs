using Cysharp.Threading.Tasks;
using PartyQuiz.Utils;

namespace PartyQuiz.Gameplay.Sequences
{
    public sealed partial class SequenceController
    {
        private void SubscribeCanAnswer()
        {
            _gameController.OnCanAnswer += OnCanAnswerHandler;
            DC.AddDisposable(() => _gameController.OnCanAnswer -= OnCanAnswerHandler);
        }
        
        private void OnCanAnswerHandler()
        {
            Play(CanAnswer).HandleExceptions();
        }

        private async UniTask CanAnswer()
        {
            var sequence = new Sequence().SetText("GO!").SetCameraType(ECameraPointType.PlayersCanAnswer);
            await Run(sequence, _hostSpeechBubble);
        }
    }
}