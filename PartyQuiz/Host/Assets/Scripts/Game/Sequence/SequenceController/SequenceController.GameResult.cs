using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Gameplay.Studio;
using UnityEngine;

namespace PartyQuiz.Gameplay.Sequences
{
    public sealed partial class SequenceController
    {
        [SerializeField] private GameResultPedestal _gameResultPedestal;
        [SerializeField] private PlayersPlatform _playersPlatform;

        private void SubscribeGameResult()
        {
            _gameController.OnSequenceSkipped += OnSequenceSkippedHandler;
            DC.AddDisposable(() => _gameController.OnSequenceSkipped -= OnSequenceSkippedHandler);
            
            _gameController.OnGameOver += OnGameOverHandler;
            DC.AddDisposable(() => _gameController.OnGameOver -= OnGameOverHandler);
        }
        
        private async void OnGameOverHandler()
        {
            _gameController.SendSequenceStartedCommand();

            await Play(GameResultSequence);
        }

        private async UniTask GameResultSequence()
        {
            var introduction = new Sequence().SetText("Nothing lasts forever, and this game is not an exception").SetCameraType(ECameraPointType.HostCloseUp);
            await Run(introduction, _hostSpeechBubble, _textReader);
            
            var explanation = new Sequence().SetText("You have made it to the very end, and now it's time to see what the results are").SetCameraType(ECameraPointType.Overall);
            await Run(explanation, _hostSpeechBubble, _textReader);
            
            _playersPlatform.DespawnEveryPlayer();

            var cameraMove = new Sequence().SetText("Let us proceed to our special results stage").SetCameraType(ECameraPointType.Overall);
            await Run(cameraMove, _hostSpeechBubble, _textReader, _gameResultPedestal);
            
            var playersShout = new Sequence().SetText("Let's see how our players did").SetCameraType(ECameraPointType.GameResult);
            await Run(playersShout, _hostSpeechBubble, _textReader, _gameResultPedestal);
            
            var orderedPlayers = _gameController.PlayersOrderedByScore;

            var winnerPlayers = new List<Player>();
            var loserPlayers = new List<Player>();

            for (var i = 0; i < orderedPlayers.Length; i++)
            {
                var player = orderedPlayers[i];

                if (i < 3)
                    winnerPlayers.Add(player);
                else
                    loserPlayers.Add(player);
            }

            Player winner = null;
            
            for (var i = 0; i < winnerPlayers.Count; i++)
            {
                var player = winnerPlayers[i];
                var text = $"{player.Name} scores {player.Score}!";

                if (i >= winnerPlayers.Count - 1)
                {
                    player.IsWinner = true;
                    winner = player;
                }

                var playerSequence = new Sequence().SetText(text).SetKey(player).SetCameraType(ECameraPointType.GameResult).SetDelay(3.0f);
                await Run(playerSequence, _hostSpeechBubble, _textReader, _gameResultPedestal);
            }

            Debug.Assert(winner != null, nameof(winner) + " != null");

            if (loserPlayers.Count > 0)
            {
                var amazingJob = new Sequence().SetText("And the other players, you did an amazing job, too!");
                await Run(amazingJob, _hostSpeechBubble, _textReader);
                
                foreach (var player in loserPlayers)
                    _gameResultPedestal.SpawnAsLoser(player);

                var thankYou = new Sequence().SetText("Thank you for playing the game!");
                await Run(thankYou, _hostSpeechBubble, _textReader);
            }

            var congratulations = new Sequence().SetText($"That is it, congratulations to the winner, {winner.Name}");
            await Run(congratulations, _hostSpeechBubble, _textReader);
                
            var seeYou = new Sequence().SetText("See you next time!");
            await Run(seeYou, _hostSpeechBubble, _textReader);
        }
    }
}