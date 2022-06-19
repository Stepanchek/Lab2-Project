using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Gameplay.UI
{
    internal sealed class UIController : UIElement, IInitializable
    {
        [SerializeField] private MainMenuScreen _mainMenuScreen;
        [SerializeField] private ResumeGameScreen _resumeGameScreen;
        [SerializeField] private NewGameScreen _newGameScreen;

        private GameController _gameController;

        private Question _catInPokeQuestion;
        private Theme _catInPokeTheme;

        [Inject]
        public void Construct(GameController gameController)
        {
            _gameController = gameController;
        }

        public void Initialize()
        {
            _mainMenuScreen.Show(OnResumeGameButtonPressedHandler, OnNewGameButtonPressedHandler);
            _resumeGameScreen.Init(OnResumeGameHandler, OnBackButtonPressedHandler);
            _newGameScreen.Init(_gameController, OnNewGameHandler, OnBackButtonPressedHandler);
        }

        private void OnResumeGameButtonPressedHandler()
        {
            _mainMenuScreen.HideGameObject();
            _resumeGameScreen.ShowGameObject();
        }

        private void OnNewGameButtonPressedHandler()
        {
            _mainMenuScreen.HideGameObject();
            _newGameScreen.ShowGameObject();
        }

        private void OnBackButtonPressedHandler()
        {
            _mainMenuScreen.ShowGameObject();
            _resumeGameScreen.HideGameObject();
            _newGameScreen.HideGameObject();
        }

        private void OnResumeGameHandler(string id)
        {
            StartGame(id);
        }

        private void OnNewGameHandler()
        {
            StartGame(string.Empty);
        }

        private void StartGame(string id)
        {
            _mainMenuScreen.Dispose();
            _resumeGameScreen.Dispose();
            _newGameScreen.Dispose();
            
            _gameController.Run(id);
        }
    }
}