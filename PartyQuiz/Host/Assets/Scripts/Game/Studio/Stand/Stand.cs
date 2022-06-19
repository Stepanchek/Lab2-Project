using Cinemachine;
using DG.Tweening;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Gameplay.VFX;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class Stand : ObjectWithDisposableContainer
    {
        private const float STAND_MOVEMENT_SPEED = 0.3f;

        [SerializeField] private TextMeshPro _nameLabel;
        [SerializeField] private TextMeshPro _scoreLabel;

        [SerializeField] private Transform _avatarPoint;
        [SerializeField] private AnswerLatencyLabel _answerLatencyLabel;

        internal Player Player { get; private set; }
        
        internal Transform OriginalPoint { get; private set; }

        private void Awake()
        {
            _nameLabel.text = string.Empty;
            _scoreLabel.text = string.Empty;
        }

        internal void Init(GameController gameController, Player player)
        {
            Player = player;

            Player.OnNameChanged += OnNameChangedHandler;
            DC.AddDisposable(() => Player.OnNameChanged -= OnNameChangedHandler);

            Player.OnScoreChanged += OnScoreChangedHandler;
            DC.AddDisposable(() => Player.OnScoreChanged -= OnScoreChangedHandler);

            Player.OnOnlineStatusChanged += OnOnlineStatusChangedHandler;
            DC.AddDisposable(() => Player.OnOnlineStatusChanged -= OnOnlineStatusChangedHandler);

            Player.OnButtonPressed += OnButtonPressedHandler;
            DC.AddDisposable(() => Player.OnButtonPressed -= OnButtonPressedHandler);

            Player.OnAvatarChanged += OnAvatarChangedHandler;
            DC.AddDisposable(() => Player.OnAvatarChanged -= OnAvatarChangedHandler);

            OnNameChangedHandler(string.Empty, player.Name);
            OnScoreChangedHandler(player, 0);
            OnOnlineStatusChangedHandler(player.IsOnline);
            OnAvatarChangedHandler();

            _answerLatencyLabel.Init(gameController, Player);
        }

        private void OnAvatarChangedHandler()
        {
            var avatar = Player.Avatar;

            if (avatar != null)
                avatar.SetPoint(_avatarPoint);
        }

        private void OnNameChangedHandler(string oldName, string newName)
        {
            _nameLabel.text = newName;
        }

        private void OnScoreChangedHandler(Player player, int diff)
        {
            if (Player.Role == ERole.Player)
                _scoreLabel.text = player.Score.ToString();
        }

        private void OnOnlineStatusChangedHandler(bool isOnline)
        {
            if (isOnline == false)
                _scoreLabel.text = "-offline-";
        }

        private void OnButtonPressedHandler(Player player)
        {
        }

        internal void SetPoint(Transform point, bool setAsOriginal)
        {
            if (setAsOriginal)
                OriginalPoint = point;

            transform.SetParentAndReset(point);
        }

        internal void ResetToOriginal()
        {
            transform.DOLocalMove(Vector3.zero, STAND_MOVEMENT_SPEED);
        }

        internal void Move(Vector3 destination)
        {
            transform.DOLocalMove(transform.localPosition + destination, STAND_MOVEMENT_SPEED);
        }
    }
}
