using Cysharp.Threading.Tasks;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;

namespace PartyQuiz.Gameplay.Sequences
{
    public sealed partial class SequenceController
    {
        private void SubscribeQuestionSelected()
        {
            _gameController.OnQuestionSelected += OnQuestionSelectedHandler;
            DC.AddDisposable(() => _gameController.OnQuestionSelected -= OnQuestionSelectedHandler);
        }
        
        private void OnQuestionSelectedHandler(Question question, Theme theme)
        {
            Play(QuestionSelected).HandleExceptions();
        }

        private async UniTask QuestionSelected()
        {
            var sequence = new Sequence().SetText("Get ready!");
            await Run(sequence, _hostSpeechBubble);
        }
    }
}