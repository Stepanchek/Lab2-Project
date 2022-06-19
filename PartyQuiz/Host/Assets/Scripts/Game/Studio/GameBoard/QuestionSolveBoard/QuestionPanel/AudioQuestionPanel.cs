using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Audio;
using PartyQuiz.Utils;
using Redcode.Awaiting;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio.Board
{
    internal sealed class AudioQuestionPanel : ObjectWithDisposableContainer, IQuestionPanel<AudioClip>
    {
        private AudioController _audioController;

        public bool CanProceed { get; private set; } = true;

        internal void Init(AudioController audioController)
        {
            _audioController = audioController;   
        }

        public void Show(AudioClip value)
        {
            CanProceed = false;
            
            _audioController.PlayQuestion(value);
            ShowGameObject();
            
            Wait().HandleExceptions();
        }

        public void SetPaused(bool isPaused)
        {
            _audioController.SetPaused(isPaused);
        }

        private async UniTask Wait()
        {
            await new WaitForSeconds(3.0f);

            CanProceed = true;
        }
        
        public override void Dispose()
        {
            _audioController.Stop();
            
            base.Dispose();
        }
    }
}