using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Audio;
using PartyQuiz.Utils;
using Redcode.Awaiting;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Gameplay.Sequences
{
    public sealed partial class SequenceController : ObjectWithDisposableContainer, IInitializable
    {
        [SerializeField] private SpeechBubble _hostSpeechBubble;

        private GameController _gameController;
        private TextReader _textReader;
        private CameraController _cameraController;

        private CancellationTokenSource _cancellationTokenSource;

        private static bool SkipSequences
        {
            get
            {
#if UNITY_EDITOR
                return false;
#else
                return false;
#endif
            }
        }

        [Inject]
        public void Construct(GameController gameController, AudioController audioController,
            CameraController cameraController)
        {
            _gameController = gameController;
            _textReader = audioController.TextReader;
            _cameraController = cameraController;
        }

        public void Initialize()
        {
            SubscribeMeetPlayers();
            SubscribeQuestionEnded();
            SubscribeQuestionSelected();
            SubscribeCanAnswer();
            SubscribeCatInPoke();
            SubscribeRoundResult();
            SubscribeRoundChanged();
            SubscribeGameResult();
            SubscribeAuction();
        }

        private async UniTask Play(Func<UniTask> task)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await UniTask.Create(task);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async UniTask Run(Sequence sequence, params ISequencePlayer[] sequencePlayers)
        {
            if (SkipSequences)
                return;

            var cameraPoint = sequence.CameraPoint;

            if (cameraPoint != ECameraPointType.None)
                _cameraController.SetCameraPointActive(cameraPoint);

            try
            {
                await UniTask.WhenAll(sequencePlayers.Select(x => x.Play(sequence)))
                    .AttachExternalCancellation(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }

            if (sequence.StopAfterDone)
            {
                foreach (var sequencePlayer in sequencePlayers)
                    sequencePlayer.Stop();
            }

            if (_cancellationTokenSource.IsCancellationRequested == false)
                await new WaitForSeconds(sequence.Delay);
        }

        public void Skip()
        {
            _cancellationTokenSource?.Cancel();
        }

        private void OnSequenceSkippedHandler()
        {
            Skip();
        }
    }
}