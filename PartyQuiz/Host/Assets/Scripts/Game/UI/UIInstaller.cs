using PartyQuiz.Gameplay.UI;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Gameplay
{
    internal sealed class UIInstaller : MonoInstaller
    {
        [SerializeField] private UIController _uiController;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UIController>().FromInstance(_uiController).AsSingle();
        }
    }
}