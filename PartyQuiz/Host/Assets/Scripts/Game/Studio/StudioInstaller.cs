using PartyQuiz.Gameplay.Sequences;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class StudioInstaller : MonoInstaller
    {
        [SerializeField] private StudioController _studioController;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private SequenceController _sequenceController;
        [SerializeField] private GameResultPedestal _gameResultPedestal;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<StudioController>().FromInstance(_studioController).AsSingle();
            Container.BindInterfacesAndSelfTo<CameraController>().FromInstance(_cameraController).AsSingle();
            Container.BindInterfacesAndSelfTo<SequenceController>().FromInstance(_sequenceController).AsSingle();
            Container.BindInterfacesAndSelfTo<GameResultPedestal>().FromInstance(_gameResultPedestal).AsSingle();
        }
    }
}