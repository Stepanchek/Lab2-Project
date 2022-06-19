using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Studio;
using UnityEngine;

namespace PartyQuiz.Gameplay.Sequences
{
    public sealed partial class SequenceController
    {
        [SerializeField] private RoundResultBoard _roundResultBoard;

        private void SubscribeRoundResult()
        {
            _gameController.OnNewRoundRequested += OnNewRoundRequestedHandler;
            DC.AddDisposable(() => _gameController.OnNewRoundRequested -= OnNewRoundRequestedHandler);
        }
        
        private async void OnNewRoundRequestedHandler()
        {
            _gameController.SendSequenceStartedCommand();

            await Play(RoundResultSequence);

            _gameController.NextRound(true);
        }

        private async UniTask RoundResultSequence()
        {
            var introduction = new Sequence().SetText("You have answered all of the questions in this round").SetCameraType(ECameraPointType.HostCloseUp);
            await Run(introduction, _hostSpeechBubble, _textReader, _roundResultBoard);
            
            var takeALook = new Sequence().SetText("Let's take a look at our score board").SetCameraType(ECameraPointType.Overall);
            await Run(takeALook, _hostSpeechBubble, _textReader, _roundResultBoard);

            for (var i = _gameController.PlayersOrderedByScore.Length - 1; i >= 0; i--)
            {
                var player = _gameController.PlayersOrderedByScore[i];
                var text = $"{player.Name} has {player.Score} points";

                var playerSequence = new Sequence().SetText(text).SetKey(player).SetCameraType(ECameraPointType.RoundResult);
                await Run(playerSequence, _hostSpeechBubble, _textReader, _roundResultBoard);
            }

            var lowestScorePlayer = _gameController.LowestScorePlayer;
            lowestScorePlayer.Avatar.CameraPoint.SetState(true);
            
            var lowestScoreSequence = new Sequence().SetText($"{lowestScorePlayer.Name} will be selecting the question now as they have the least score points");
            await Run(lowestScoreSequence, _hostSpeechBubble, _textReader);
            
            lowestScorePlayer.Avatar.CameraPoint.SetState(false);
            
            var nextRound = new Sequence().SetText("Let's begin the next round, shall we?").SetCameraType(ECameraPointType.Overall);
            await Run(nextRound, _hostSpeechBubble, _textReader);
        }
    }
}