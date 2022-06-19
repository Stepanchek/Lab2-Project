using PartyQuiz.Gameplay.Players;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Gameplay
{
    public sealed class CameraController : ObjectWithDisposableContainer, IInitializable
    {
        [SerializeField] private CameraPointsDictionary _cameraPoints;

        private GameController _gameController;

        [Inject]
        public void Construct(GameController gameController)
        {
            _gameController = gameController;
        }

        public void Initialize()
        {
            _gameController.OnQuestionSelected += OnQuestionSelectedHandler;
            DC.AddDisposable(() => _gameController.OnQuestionSelected -= OnQuestionSelectedHandler);

            _gameController.OnCatInPokeOwnerSelected += OnCatInPokeOwnerSelectedHandler;
            DC.AddDisposable(() => _gameController.OnCatInPokeOwnerSelected -= OnCatInPokeOwnerSelectedHandler);

            _gameController.OnPlayerStoppedAnswering += OnPlayerStoppedAnsweringHandler;
            DC.AddDisposable(() => _gameController.OnPlayerStoppedAnswering -= OnPlayerStoppedAnsweringHandler);

            SetCameraPointActive(ECameraPointType.Overall);
        }

        private void OnQuestionSelectedHandler(Question question, Theme theme)
        {
            if (question.Type.Value == EQuestionType.Normal)
                SetCameraPointActive(ECameraPointType.QuestionCloseUp);
        }

        private void OnCatInPokeOwnerSelectedHandler(Player player)
        {
            SetCameraPointActive(ECameraPointType.QuestionCloseUp);
        }

        private void OnPlayerStoppedAnsweringHandler(Player player, bool wasCorrect)
        {
            if (wasCorrect)
                SetCameraPointActive(ECameraPointType.QuestionCloseUp);
        }

        internal void SetCameraPointActive(ECameraPointType type)
        {
            foreach (var (cameraType, cameraPoint) in _cameraPoints)
                cameraPoint.SetState(cameraType == type);
        }
    }
}