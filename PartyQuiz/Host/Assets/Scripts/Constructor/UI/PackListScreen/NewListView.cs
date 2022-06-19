using System;
using PartyQuiz.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PartyQuiz.Constructor
{
    internal sealed class NewListView : UIElement
    {
        [SerializeField] private Button _createButton;
        
        private Action _onNewEntityCreated;

        internal void Show(Action onNewEntityCreated)
        {
            DC.Dispose();
            
            _onNewEntityCreated = onNewEntityCreated;

            _createButton.onClick.AddListener(OnCreateButtonPressedHandler);
            DC.AddDisposable(() => _createButton.onClick.RemoveListener(OnCreateButtonPressedHandler));
            
            ShowGameObject();
        }

        private void OnCreateButtonPressedHandler()
        {
            _onNewEntityCreated?.Invoke();
        }

        internal void SetAsLast()
        {
            transform.SetAsLastSibling();
        }
    }
}