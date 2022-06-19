using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PartyQuiz.Utils;
using Redcode.Awaiting;
using UnityEngine;

namespace PartyQuiz.Gameplay.VFX
{
    internal sealed class PointParticle : Poolable<EPointParticleType, PointParticlePool>
    {
        [SerializeField] private List<ParticleSystem> _particleSystems;
        [SerializeField] private float _lifeTime = 1.0f;

        internal async UniTask Animate(Vector3 point)
        {
            transform.position = point;

            gameObject.SetActive(true);

            foreach (var system in _particleSystems)
                system.Play();

            await TryCountLifetime();
        }

        private async UniTask TryCountLifetime()
        {
            if (_lifeTime <= 0)
                return;

            await new WaitForSeconds(_lifeTime);
            Despawn();
        }

        private void Despawn()
        {
            gameObject.SetActive(false);

            foreach (var system in _particleSystems)
                system.Stop();

            Pool.Despawn(this);
        }
    }
}