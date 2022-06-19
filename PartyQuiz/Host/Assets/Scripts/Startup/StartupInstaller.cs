using UnityEngine;
using Zenject;

namespace PartyQuiz.Gameplay
{
    internal sealed class StartupInstaller : MonoInstaller
    {
        [SerializeField] private Startup _startup;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Startup>().FromInstance(_startup).AsSingle();
        }
    }
}