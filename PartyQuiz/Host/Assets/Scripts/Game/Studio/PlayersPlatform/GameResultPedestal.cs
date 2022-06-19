using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Gameplay.Sequences;
using PartyQuiz.Gameplay.VFX;
using PartyQuiz.Utils;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class GameResultPedestal : ObjectWithDisposableContainer, ISequencePlayer
    {
        [SerializeField] private PlayerPointsPool _winnersPointsPool;
        [SerializeField] private PlayerPointsPool _losersPointsPool;

        [SerializeField] private PointParticle _confettiParticle;
        [SerializeField] private PointParticle _smokeParticle;

        private PointParticlePool _pool;

        [Inject]
        public void Construct(PointParticlePool pool)
        {
            _pool = pool;
        }

        public async UniTask Play(Sequence sequence)
        {
            if (sequence.Key is not Player player)
                return;

            var point = _winnersPointsPool.Request(player);
            player.Avatar.SetPoint(point);

            var avatarPosition = player.Avatar.transform.position;

            var smoke = _pool.Spawn(_smokeParticle);
            smoke.Animate(avatarPosition).HandleExceptions();

            if (player.IsWinner)
            {
                var confetti = _pool.Spawn(_confettiParticle);
                confetti.Animate(avatarPosition).HandleExceptions();
            }

            await UniTask.Yield();
        }

        public void Stop()
        {
        }

        internal void SpawnAsLoser(Player player)
        {
            var point = _losersPointsPool.Request(player);
            player.Avatar.SetPoint(point);
        }
    }
}