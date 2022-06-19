using System;
using PartyQuiz.Gameplay.VFX;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;

namespace PartyQuiz.Gameplay.Players
{
    public sealed class Avatar : ObjectWithDisposableContainer
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AvatarData _avatarData;

        private static readonly int _handsOnPanel = Animator.StringToHash("HandsOnPanel");
        private static readonly int _greetings = Animator.StringToHash("Greetings");
        private static readonly int _greetingsIndex = Animator.StringToHash("Greetings_Index");
        private static readonly int _buttonPushed = Animator.StringToHash("ButtonPushed");
        private static readonly int _rightAnswer = Animator.StringToHash("RightAnswer");
        private static readonly int _wrongAnswer = Animator.StringToHash("WrongAnswer");
        private static readonly int _answers = Animator.StringToHash("Answers");
        private static readonly int _catInPoke = Animator.StringToHash("CatInPoke");
        private static readonly int _pointAtBoard = Animator.StringToHash("PointAtBoard");

        private Player _player;
        private static readonly int _emoji = Animator.StringToHash("Emoji");

        internal PlayerCameraPoint CameraPoint { get; private set; }
        internal Transform EmojiPoint { get; private set; }
        internal TextMeshPro SpeechBubble { get; private set; }

        internal void Init(Player player, GameController gameController)
        {
            _player = player;

            SetAnimatorControllerByRole(_player.Role);

            CameraPoint = Instantiate(_avatarData.CameraPoint, transform);
            EmojiPoint = Instantiate(_avatarData.EmojiPoint, transform).transform;
            SpeechBubble = Instantiate(_avatarData.SpeechBubble, transform, worldPositionStays: false);

            SpeechBubble.gameObject.SetActive(false);

            Subscribe(gameController);

            _animator.SetBool(_handsOnPanel, true);
        }

        private void SetAnimatorControllerByRole(ERole role)
        {
            switch (role)
            {
                case ERole.Player:
                {
                    _animator.runtimeAnimatorController = _avatarData.PlayerAnimator;
                    break;
                }
                case ERole.Host:
                {
                    _animator.runtimeAnimatorController = _avatarData.HostAnimator;
                    break;
                }
                case ERole.NotSet:
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }
        }

        private void Subscribe(GameController gameController)
        {
            _player.OnButtonPressed += OnPlayerButtonPressedHandler;
            DC.AddDisposable(() => _player.OnButtonPressed -= OnPlayerButtonPressedHandler);

            gameController.OnPlayerStartedAnswering += OnPlayerStartedAnsweringHandler;
            DC.AddDisposable(() => gameController.OnPlayerStartedAnswering -= OnPlayerStartedAnsweringHandler);

            gameController.OnPlayerStoppedAnswering += OnPlayerStoppedAnsweringHandler;
            DC.AddDisposable(() => gameController.OnPlayerStoppedAnswering -= OnPlayerStoppedAnsweringHandler);

            gameController.OnCatInPokeOwnerSelected += OnCatInPokeOwnerSelectedHandler;
            DC.AddDisposable(() => gameController.OnCatInPokeOwnerSelected -= OnCatInPokeOwnerSelectedHandler);

            gameController.OnQuestionDisplayed += OnQuestionDisplayedHandler;
            DC.AddDisposable(() => gameController.OnQuestionDisplayed -= OnQuestionDisplayedHandler);

            gameController.OnQuestionEnded += OnQuestionEndedHandler;
            DC.AddDisposable(() => gameController.OnQuestionEnded -= OnQuestionEndedHandler);

            gameController.OnNewRoundRequested += OnNewRoundRequestedHandler;
            DC.AddDisposable(() => gameController.OnNewRoundRequested -= OnNewRoundRequestedHandler);

            gameController.OnRoundChanged += OnRoundChangedHandler;
            DC.AddDisposable(() => gameController.OnRoundChanged -= OnRoundChangedHandler);

            CameraPoint.OnStateChanged += OnCameraStateChangedHandler;
            DC.AddDisposable(() => CameraPoint.OnStateChanged -= OnCameraStateChangedHandler);

            gameController.OnAuctionCalled += OnAuctionCalledHandler;
            DC.AddDisposable(() => gameController.OnAuctionCalled -= OnAuctionCalledHandler);

            gameController.OnAuctionFinished += OnAuctionFinishedHandler;
            DC.AddDisposable(() => gameController.OnAuctionFinished -= OnAuctionFinishedHandler);

            gameController.OnBidMade += OnBidMadeHandler;
            DC.AddDisposable(() => gameController.OnBidMade -= OnBidMadeHandler);

            gameController.OnPlayerPassesAuction += OnPlayerPassesAuctionHandler;
            DC.AddDisposable(() => gameController.OnPlayerPassesAuction -= OnPlayerPassesAuctionHandler);
        }

        internal void SetPoint(Transform point)
        {
            transform.SetParentAndReset(point);
        }

        private void OnPlayerButtonPressedHandler(Player player)
        {
            _animator.SetTrigger(_buttonPushed);
        }

        private void OnPlayerStartedAnsweringHandler(Player player)
        {
            if (_player == player || _player.Role == ERole.Host)
                _animator.SetBool(_answers, true);
        }

        private void OnPlayerStoppedAnsweringHandler(Player player, bool wasCorrect)
        {
            if (_player == player || _player.Role == ERole.Host)
            {
                _animator.SetTrigger(wasCorrect ? _rightAnswer : _wrongAnswer);
                _animator.SetBool(_answers, false);
            }
        }

        private void OnCameraStateChangedHandler(bool isEnabled, bool playAnimation)
        {
            if (isEnabled && playAnimation)
            {
                _animator.SetInteger(_greetingsIndex, UnityEngine.Random.Range(0, 5));
                _animator.SetTrigger(_greetings);
            }
        }

        private void OnCatInPokeOwnerSelectedHandler(Player player)
        {
            if (_player == player || _player.Role == ERole.Host)
                _animator.SetTrigger(_catInPoke);
        }

        private void OnQuestionDisplayedHandler(Question question, Theme theme)
        {
            if (_player.Role == ERole.Host)
                _animator.SetBool(_pointAtBoard, false);
        }

        private void OnNewRoundRequestedHandler()
        {
            if (_player.Role == ERole.Host)
                _animator.SetBool(_pointAtBoard, false);
        }

        private void OnRoundChangedHandler(Round round)
        {
            if (round.Type == ERoundType.Final)
                return;

            if (_player.Role == ERole.Host)
                _animator.SetBool(_pointAtBoard, true);
        }

        private void OnQuestionEndedHandler()
        {
            if (_player.Role == ERole.Host)
                _animator.SetBool(_pointAtBoard, true);
        }

        private void OnBidMadeHandler(Player bidder, int bid)
        {
            if (bidder == _player)
            {
                SpeechBubble.text = $"{bid}!";
                SpeechBubble.color = _avatarData.BidColor;
            }
            else
            {
                if (_player.Score <= bid)
                {
                    SpeechBubble.text = ":(";
                    SpeechBubble.color = _avatarData.PassColor;
                }
                else
                {
                    SpeechBubble.color = _avatarData.UndecidedColor;
                }
            }
        }

        private void OnPlayerPassesAuctionHandler(Player player)
        {
            if (player != _player)
                return;

            SpeechBubble.text = "I'll pass";
            SpeechBubble.color = _avatarData.PassColor;
        }

        private void OnAuctionCalledHandler(Question question, Theme theme, Player selector)
        {
            if (_player.Role == ERole.Host)
                return;

            SpeechBubble.gameObject.SetActive(true);
            SpeechBubble.text = "?";
            SpeechBubble.color = _avatarData.UndecidedColor;

            OnBidMadeHandler(selector, question.Price.Value);
        }

        private void OnAuctionFinishedHandler(Player highestBidder, int highestBid)
        {
            SpeechBubble.gameObject.SetActive(false);
        }

        internal void SetEmojiPlayedState(EPointParticleType emojiType, bool value)
        {
            var parameter = value ? (int)emojiType : -1;
            _animator.SetInteger(_emoji, parameter);
        }
    }
}