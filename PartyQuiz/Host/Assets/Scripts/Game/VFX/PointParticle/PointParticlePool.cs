using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Gameplay.VFX
{
    internal sealed class PointParticlePool : ObjectPool<PointParticle>
    {
        protected override void ForceCreate(PointParticle value)
        {
            var item = Object.Instantiate(value);
            Despawn(item);
        }
    }
}