using UnityEngine;
using Zenject;

namespace PartyQuiz.Debug
{
    internal sealed class ConsoleInstaller : MonoInstaller
    {
        [SerializeField] private Console _console;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Console>().FromInstance(_console).AsSingle();
        }
    }
}