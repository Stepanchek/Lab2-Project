using Cysharp.Threading.Tasks;

namespace PartyQuiz.Gameplay.Sequences
{
    internal interface ISequencePlayer
    {
        UniTask Play(Sequence sequence);
        void Stop();
    }
}