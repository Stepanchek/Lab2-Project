using System.Linq;
using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Players;

namespace PartyQuiz.Gameplay.Sequences
{
    public sealed partial class SequenceController
    {
        private void SubscribeMeetPlayers()
        {
            _gameController.OnAllHereCalled += OnAllHereCalledHandler;
            DC.AddDisposable(() => _gameController.OnAllHereCalled -= OnAllHereCalledHandler);
        }
        
        private async void OnAllHereCalledHandler()
        {
            _gameController.SendSequenceStartedCommand();

            await Play(MeetPlayersSequence);

            _gameController.NextRound(false);
        }

        private async UniTask MeetPlayersSequence()
        {
            var welcome = new Sequence().SetText("Hello everyone and welcome to Party Quiz!").SetCameraType(ECameraPointType.HostCloseUp);
            await Run(welcome, _hostSpeechBubble, _textReader);
            
            var meet = new Sequence().SetText("Let's meet our dear players!").SetCameraType(ECameraPointType.Overall);
            await Run(meet, _hostSpeechBubble, _textReader);

            foreach (var (_, player) in _gameController.Players)
            {
                if (player.Role != ERole.Player)
                    continue;

                player.Avatar.CameraPoint.SetState(true, true);
                
                var playerSequence = new Sequence().SetText(player.Name).SetDelay(2.1f);
                await Run(playerSequence, _hostSpeechBubble, _textReader);

                player.Avatar.CameraPoint.SetState(false);
            }

            var hostName = _gameController.Players.Single(x => x.Value.Role == ERole.Host).Value.Name;
            
            var hostSequence = new Sequence().SetText($"And of course me, your humble host {hostName}").SetCameraType(ECameraPointType.HostCloseUp);
            await Run(hostSequence, _hostSpeechBubble, _textReader);
            
            var selector = _gameController.Selector;

            selector.Avatar.CameraPoint.SetState(true);
            
            var selectorSequence = new Sequence().SetText($"{selector.Name} will be selecting the question first");
            await Run(selectorSequence, _hostSpeechBubble, _textReader);

            selector.Avatar.CameraPoint.SetState(false);
            
            var begin = new Sequence().SetText("And with that, we begin our first round!").SetCameraType(ECameraPointType.Overall);
            await Run(begin, _hostSpeechBubble, _textReader);
        }
    }
}