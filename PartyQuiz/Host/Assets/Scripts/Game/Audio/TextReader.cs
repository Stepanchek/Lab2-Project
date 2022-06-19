using Crosstales.RTVoice;
using Crosstales.RTVoice.Model;
using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Sequences;
using PartyQuiz.Utils;
using UnityEngine;
using Sequence = PartyQuiz.Gameplay.Sequences.Sequence;

namespace PartyQuiz.Gameplay.Audio
{
    internal sealed class TextReader : ObjectWithDisposableContainer, ISequencePlayer
    {
        private const float DEFAULT_READING_RATE = 1.3f;
        
        [SerializeField] private AudioSource _voiceAudioSource;

        private Speaker _speaker;
        private string _uid;

        internal bool HasFinished { get; private set; } = true;

        internal void Init()
        {
            _speaker = Speaker.Instance;

            _speaker.OnSpeakComplete += OnSpeakCompletedHandler;
            DC.AddDisposable(() => _speaker.OnSpeakComplete -= OnSpeakCompletedHandler);

            _speaker.OnErrorInfo += OnErrorHandler;
            DC.AddDisposable(() => _speaker.OnErrorInfo -= OnErrorHandler);
        }

        public async UniTask Play(Sequence sequence)
        {
            await Read(sequence.Text);
        }
        
        internal async UniTask Read(string text, float rate = DEFAULT_READING_RATE)
        {
            var culture = text.DetermineCulture();
            await Read(text, culture, rate);
        }

        private async UniTask Read(string text, string culture, float rate = DEFAULT_READING_RATE)
        {
            HasFinished = false;

            var voice = _speaker.VoiceForCulture(culture);
            _uid = _speaker.Speak(text, _voiceAudioSource, voice, true, rate);

            await new WaitUntil(() => HasFinished);
        }

        /// <summary>
        /// The question was fully pronounced
        /// </summary>
        private void OnSpeakCompletedHandler(Wrapper wrapper)
        {
            if (wrapper.Uid != _uid)
                return;

            Stop();
        }

        /// <summary>
        /// In case of pronunciation error, we still need to finish the talking sequence
        /// </summary>
        private void OnErrorHandler(Wrapper wrapper, string info)
        {
            if (wrapper.Uid != _uid)
                return;

            Stop();
        }

        public void Stop()
        {
            HasFinished = true;
        }
    }
}