using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Structure.Runtime;

namespace PartyQuiz.Gameplay.Sequences
{
    public sealed partial class SequenceController
    {
        private bool _wasCatInPokePlayed;

        private void SubscribeCatInPoke()
        {
            _gameController.OnCatInPokeCalled += CatInPokeCalledHandler;
            DC.AddDisposable(() => _gameController.OnCatInPokeCalled -= CatInPokeCalledHandler);

            _gameController.OnCatInPokeOwnerSelected += OnCatInPokeOwnerSelectedHandler;
            DC.AddDisposable(() => _gameController.OnCatInPokeOwnerSelected -= OnCatInPokeOwnerSelectedHandler);
        }
        
        private async void CatInPokeCalledHandler(Question question, Theme theme)
        {
            _gameController.SendSequenceStartedCommand();

            await Play(CatInPokeSequence);

            _gameController.ConfirmCatInPoke();
            _wasCatInPokePlayed = true;
        }

        private async UniTask CatInPokeSequence()
        {
            if (_wasCatInPokePlayed == false)
            {
                var introduction = new Sequence().SetText("You've got a cat in a poke").SetCameraType(ECameraPointType.HostCloseUp);
                await Run(introduction, _hostSpeechBubble, _textReader);

                var explanation = new Sequence().SetText("You have to give that question away to somebody else");
                await Run(explanation, _hostSpeechBubble, _textReader);

                var overall = new Sequence().SetText("That person will be obliged to answer that question").SetCameraType(ECameraPointType.Overall);
                await Run(overall, _hostSpeechBubble, _textReader);

                var decision = new Sequence().SetText("Tell me, who do you want to hand over the question to?").SetCameraType(ECameraPointType.CatInPoke).SetStopAfterDone(false);
                await Run(decision, _hostSpeechBubble, _textReader);
            }
            else
            {
                var explanation = new Sequence().SetText("Oh, there's another cat!").SetCameraType(ECameraPointType.HostCloseUp);
                await Run(explanation, _hostSpeechBubble, _textReader);

                var decision = new Sequence().SetText("You know the drill").SetCameraType(ECameraPointType.CatInPoke).SetStopAfterDone(false);
                await Run(decision, _hostSpeechBubble, _textReader);
            }
        }

        private void OnCatInPokeOwnerSelectedHandler(Player player)
        {
            _hostSpeechBubble.Stop();
        }
    }
}