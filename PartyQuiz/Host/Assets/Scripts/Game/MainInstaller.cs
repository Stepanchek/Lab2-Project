using PartyQuiz.Gameplay.Audio;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Gameplay.VFX;
using PartyQuiz.Network;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Gameplay
{
    internal sealed class MainInstaller : MonoInstaller
    {
        [SerializeField] private GameController _gameController;
        [SerializeField] private AudioController _audioController;
        [SerializeField] private AvatarFactory _avatarFactory;
        [SerializeField] private EmojiCreator _emojiCreator;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameController>().FromInstance(_gameController).AsSingle();
            Container.BindInterfacesAndSelfTo<AudioController>().FromInstance(_audioController).AsSingle();
            Container.BindInterfacesAndSelfTo<AvatarFactory>().FromInstance(_avatarFactory).AsSingle();
            Container.BindInterfacesAndSelfTo<EmojiCreator>().FromInstance(_emojiCreator).AsSingle();

            Container.BindInterfacesAndSelfTo<SocketServer>().AsSingle();
            Container.BindInterfacesAndSelfTo<MessageReader>().AsSingle();
            Container.BindInterfacesAndSelfTo<MessageWriter>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<PointParticlePool>().AsSingle();
        }
    }
}