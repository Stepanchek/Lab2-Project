using UnityEngine;

namespace PartyQuiz.Gameplay.Audio
{
    [CreateAssetMenu(menuName = "Scriptable objects/New audio data")]
    internal sealed class AudioData : ScriptableObject
    {
        [SerializeField] internal AudioClip[] PositiveStings;
        [SerializeField] internal AudioClip[] NegativeStings;
        [SerializeField] internal AudioClip[] Applause;
        [SerializeField] internal AudioClip NextRound;
        [SerializeField] internal AudioClip CatInPoke;
        [SerializeField] internal AudioClip Auction;
    }
}