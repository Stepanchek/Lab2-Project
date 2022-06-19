using System;
using PartyQuiz.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PartyQuiz.Constructor
{
    internal sealed class MessageWindow : UIElement
    {
        [SerializeField] private Button _firstButton;
        [SerializeField] private Button _secondButton;

        [SerializeField] private Button _closeButton;

        internal void Show(Action onFirst, Action onSecond)
        {
            var onImport = _firstButton.OnClickAsObservable().Subscribe(_ =>
            {
                onFirst?.Invoke();
                Dispose();
            });
            
            DC.AddDisposable(onImport);
            
            var onCreate = _secondButton.OnClickAsObservable().Subscribe(_ =>
            {
                onSecond?.Invoke();
                Dispose();
            });
            
            DC.AddDisposable(onCreate);

            var onClose = _closeButton.OnClickAsObservable().Subscribe(_ => Dispose());
            DC.AddDisposable(onClose);
            
            ShowGameObject();
        }
    }
}