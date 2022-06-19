using PartyQuiz.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PartyQuiz.Gameplay
{
    internal sealed class Startup : MonoBehaviour
    {
        private ZenjectSceneLoader _sceneLoader;

        [Inject]
        public void Construct(ZenjectSceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private void Awake()
        {
            GetCultureByString.TryLoadIdentifier();
            
            _sceneLoader.LoadSceneAsync("Main", LoadSceneMode.Single);
            _sceneLoader.LoadSceneAsync("UI", LoadSceneMode.Additive);
            _sceneLoader.LoadSceneAsync("Studio", LoadSceneMode.Additive);
            _sceneLoader.LoadSceneAsync("Debug", LoadSceneMode.Additive);
        }
    }
}