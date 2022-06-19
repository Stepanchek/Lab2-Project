using Cysharp.Threading.Tasks;
using PartyQuiz.Utils.TextTyper;
using UnityEngine;

namespace PartyQuiz.Gameplay.Sequences
{
    internal sealed class SpeechBubble : MonoBehaviour, ISequencePlayer
    {
        [SerializeField] private TextTyper _textLabel;

        private void Awake()
        {
            Stop();
        }

        public async UniTask Play(Sequence sequence)
        {
            await _textLabel.Type(sequence.Text);
        }

        public void Stop()
        {
            _textLabel.Clear();
        }
    }
}