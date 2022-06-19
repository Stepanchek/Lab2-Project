using UnityEngine;
using Zenject;

namespace PartyQuiz.Constructor
{
    internal sealed class ConstructorInstaller : MonoInstaller
    {
        [SerializeField] private ConstructorController _constructorController;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ConstructorController>().FromInstance(_constructorController).AsSingle();
        }
    }
}