using System;
using PartyQuiz.Gameplay.Studio;
using PartyQuiz.Gameplay.VFX;
using PartyQuiz.Network;

namespace PartyQuiz.Gameplay.Players
{
    public sealed partial class Player
    {
        internal event Action<string, string> OnNameChanged;
        internal event Action<Player, int> OnScoreChanged;
        internal event Action<bool> OnOnlineStatusChanged;
        internal event Action<Player> OnRoleChanged;
        internal event Action OnAvatarChanged;
        internal event Action<Player> OnButtonPressed;
        internal event Action<bool> OnSetAsSelector;
        internal event Action<Player, EPointParticleType> OnEmojiPlayed;

        private PlayerData _playerData;

        private readonly MessageWriter _messageWriter;
        private readonly GameController _gameController;

        internal int? Bid;
        internal bool Pass;

        internal string Id => _playerData.Id;

        internal string Name
        {
            get => _playerData.Name;
            private set
            {
                var oldName = _playerData.Name;

                _playerData.Name = value;
                OnNameChanged?.Invoke(oldName, value);
            }
        }

        public int Score
        {
            get => _playerData.Score;
            set
            {
                var difference = value - _playerData.Score;

                _playerData.Score = value;

#if DEBUG
                if (_messageWriter != null)
#endif
                    _messageWriter.SendObjectWithTargetPlayer(Id, new
                    {
                        Command = "ScoreChanged",
                        Score = value,
                    });

                OnScoreChanged?.Invoke(this, difference);
            }
        }

        internal bool IsOnline
        {
            get => _playerData.IsOnline;
            set
            {
                _playerData.IsOnline = value;

                OnOnlineStatusChanged?.Invoke(IsOnline);
                OnNameChanged?.Invoke(Name, Name);

                if (value == false)
                    OnScoreChanged?.Invoke(this, 0);
            }
        }

        public ERole Role
        {
            get => _playerData.Role;
            set
            {
                _playerData.Role = value;
                OnRoleChanged?.Invoke(this);
            }
        }

        internal bool IsSelector
        {
            get => _playerData.IsSelector;
            set
            {
                _playerData.IsSelector = value;
                OnSetAsSelector?.Invoke(value);
            }
        }

        internal bool TriedToAnswer { get; set; }
        internal bool IsWinner { get; set; }

        internal Avatar Avatar { get; private set; }
        internal Stand Stand { get; private set; }

        internal Player(string id, bool isOnline, MessageWriter messageWriter, GameController gameController)
        {
            _playerData = new PlayerData(id, isOnline);
            _messageWriter = messageWriter;
            _gameController = gameController;
        }

#if DEBUG
        public Player(string name, int score, bool isOnline, ERole role, bool isSelector, string id, GameController gameController)
        {
            _gameController = gameController;
            _playerData = new PlayerData(name, score, isOnline, role, isSelector, id);
        }

        public void ImitateButtonPressed()
        {
            ButtonPressed(string.Empty);
        }
#endif

        public void SetAvatar(Avatar avatar)
        {
            Avatar = avatar;
            Avatar.Init(this, _gameController);
            
            OnAvatarChanged?.Invoke();
        }

        internal void SetStand(Stand stand)
        {
            Stand = stand;
        }
    }
}