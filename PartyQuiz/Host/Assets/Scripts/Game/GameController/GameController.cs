using System;
using System.Linq;
using JetBrains.Annotations;
using PartyQuiz.Gameplay.Audio;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Network;
using PartyQuiz.Serialization;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Gameplay
{
    public sealed partial class GameController : MonoBehaviour
    {
        internal event Action OnAllHereCalled;
        internal event Action OnCanAnswer;
        internal event Action OnSequenceSkipped;

        private SocketServer _socketServer;
        private MessageWriter _messageWriter;
        private AvatarFactory _avatarFactory;
        private AudioController _audioController;

        private Pack _pack;

        internal readonly ReactiveProperty<string> GameId = new();
        internal Round CurrentRound;

        internal int MaxRoundsCount => _pack.Rounds.Count;
        public int CurrentRoundIndex { get; private set; } = -1;

        internal bool AreAllQuestionsAnswered
        {
            get
            {
                var allQuestionsInCurrentRound = CurrentRound
                    .Themes
                    .SelectMany(x => x.Questions);

                return allQuestionsInCurrentRound.All(x => x.WasAnswered);
            }
        }

        [Inject]
        public void Construct(SocketServer socketServer, MessageWriter messageWriter, AvatarFactory avatarFactory,
            AudioController audioController)
        {
            _socketServer = socketServer;
            _messageWriter = messageWriter;
            _avatarFactory = avatarFactory;
            _audioController = audioController;
        }

        internal void Run([CanBeNull] string id)
        {
            GameId.Value = string.IsNullOrEmpty(id) ? RandomIdGenerator.GenerateRandomId() : id;
            _socketServer.Run(GameId.Value).HandleExceptions();
        }

        internal void LoadPq(string path)
        {
            _pack = Helper.Import(path);
        }

        public void NotifyAllHere()
        {
            var randomSelector = Players.Values.Where(x => x.Role == ERole.Player).PickRandom();
            SetPlayerAsSelector(randomSelector);

            OnAllHereCalled?.Invoke();
        }

        /// <summary>
        /// Continue the game after a question was answered or skipped
        /// </summary>
        internal void ContinueGame()
        {
            foreach (var (id, _) in Players)
            {
                _messageWriter.SendObjectWithTargetPlayer(id, new
                {
                    Command = "ContinueGame",
                });
            }
        }

        internal void SendSequenceStartedCommand()
        {
            foreach (var (id, _) in Players)
            {
                _messageWriter.SendObjectWithTargetPlayer(id, new
                {
                    Command = "SequenceStarted",
                });
            }
        }

        internal void SkipSequence()
        {
            OnSequenceSkipped?.Invoke();
        }
    }
}