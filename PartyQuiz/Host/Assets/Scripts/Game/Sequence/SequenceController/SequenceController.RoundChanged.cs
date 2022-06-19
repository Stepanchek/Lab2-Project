using Cysharp.Threading.Tasks;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;

namespace PartyQuiz.Gameplay.Sequences
{
    public sealed partial class SequenceController
    {
        private void SubscribeRoundChanged()
        {
            _gameController.OnRoundChanged += OnRoundChangedHandler;
            DC.AddDisposable(() => _gameController.OnRoundChanged -= OnRoundChangedHandler);
        }
        
        private void OnRoundChangedHandler(Round round)
        {
            Play(() => RoundChanged(round)).HandleExceptions();
        }

        private async UniTask RoundChanged(Round round)
        {
            if (round.Type == ERoundType.Final)
                return;

            await DeclareSelector();
        }
    }
}