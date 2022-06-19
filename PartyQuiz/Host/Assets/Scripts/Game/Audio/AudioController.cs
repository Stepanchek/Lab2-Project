using PartyQuiz.Utils;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Gameplay.Audio
{
    public sealed class AudioController : MonoBehaviour, IInitializable
    {
        [SerializeField] private AudioData _audioData;
        
        [SerializeField] private AudioSource _questionsAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private AudioSource _applauseAudioSource;
        
        [SerializeField] private TextReader _textReader;

        internal TextReader TextReader => _textReader;

        public void Initialize()
        {
            _textReader.Init();
        }

        internal void PlayCorrectAnswer(int questionIndex)
        {
            var clip = _audioData.PositiveStings.PickRandom();
            _sfxAudioSource.PlayOneShot(clip);

            var clapSfx = _audioData.Applause[questionIndex];
            _applauseAudioSource.PlayOneShot(clapSfx);
        }
        
        internal void PlayIncorrectAnswer()
        {
            var clip = _audioData.NegativeStings.PickRandom();
            _sfxAudioSource.PlayOneShot(clip);
        }

        internal void PlayNextRoundSfx()
        {
            _sfxAudioSource.PlayOneShot(_audioData.NextRound);
        }

        internal void PlayCatInPoke()
        {
            _sfxAudioSource.PlayOneShot(_audioData.CatInPoke);
        }

        internal void PlayAuction()
        {
            _sfxAudioSource.PlayOneShot(_audioData.Auction);
        }
        
        internal void PlayQuestion(AudioClip clip)
        {
            _questionsAudioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Pauses the question audio clip 
        /// </summary>
        internal void SetPaused(bool isPaused)
        {
            if (isPaused)
                _questionsAudioSource.Pause();
            else
                _questionsAudioSource.UnPause();
        }

        internal void Stop()
        {
            _questionsAudioSource.Stop();
        }
    }
}