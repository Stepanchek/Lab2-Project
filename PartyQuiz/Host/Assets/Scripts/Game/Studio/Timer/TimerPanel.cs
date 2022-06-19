using System;
using System.Collections;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio
{
    internal class TimerPanel : ObjectWithDisposableContainer
    {
        [SerializeField] private TextMeshPro _timeLeftLabel;

        internal event Action OnTimerEnded;

        private Coroutine _timerCoroutine;
        private float _duration;

        private bool _isTicking;

        protected TextMeshPro TimeLeftLabel => _timeLeftLabel;

        private void Awake()
        {
            _timeLeftLabel.text = string.Empty;
            
            HideGameObject();
        }
        
        /// <summary>
        /// Starts the timer over
        /// </summary>
        internal void Run(float time)
        {
            _duration = time;

            Unpause();
            ShowGameObject();

            _timerCoroutine = StartCoroutine(Co_UpdateTimer());
        }

        /// <summary>
        /// Temporarily stops the timer
        /// </summary>
        internal void Pause()
        {
            _isTicking = false;
        }

        /// <summary>
        /// Resumes the timer
        /// </summary>
        internal void Unpause()
        {
            _isTicking = true;
        }

        private IEnumerator Co_UpdateTimer()
        {
            var passedTime = 0f;

            while (passedTime <= _duration)
            {
                if (_isTicking)
                {
                    passedTime += Time.deltaTime;
                    var remainingTime = _duration - passedTime;

                    DisplayTime(remainingTime);
                }

                yield return null;
            }

            NotifyTimerEnded();
        }

        protected virtual void NotifyTimerEnded()
        {
            OnTimerEnded?.Invoke();
        }

        private void DisplayTime(float time)
        {
            TimeLeftLabel.text = time.ToString("0.0");
        }

        /// <summary>
        /// Completely stops the timer, closing the screen
        /// </summary>
        internal void Stop()
        {
            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            Pause();
            HideGameObject();
        }
    }
}