using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using PartyQuiz.Utils;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PartyQuiz.Gameplay.UI
{
    internal sealed class LoadPackPanel : UIElement
    {
        [SerializeField] private TextMeshProUGUI _packNameLabel;
        [SerializeField] private TextMeshProUGUI _errorLabel;

        [SerializeField] private Button _loadPackButton;
        [SerializeField] private CanvasGroup _newGameButtonCanvasGroup;

        private event Action<string> _onPackLoaded; 

        internal void Show(Action<string> onPackLoaded)
        {
            _onPackLoaded = onPackLoaded;
            
            _loadPackButton.onClick.AddListener(OnLoadPackButtonPressedHandler);

            UpdatePackDirectory(string.Empty);
            ShowGameObject();
        }

        private async void OnLoadPackButtonPressedHandler()
        {
            UpdatePackDirectory(string.Empty);
            _packNameLabel.text = "Loading...";

            await UniTask.Yield();

            var files = StandaloneFileBrowser.OpenFilePanel("Open pack file", "", "pq", false);

            if (files.Length <= 0)
            {
                UpdatePackDirectory(string.Empty);
                return;
            }
            
            var packDirectory = files[0];
            UpdatePackDirectory(packDirectory);
            
            _onPackLoaded?.Invoke(packDirectory);
        }

        private void UpdatePackDirectory([CanBeNull] string packDirectory)
        {
            var hasDirectory = !string.IsNullOrEmpty(packDirectory);
            
            if (hasDirectory)
            {
                _newGameButtonCanvasGroup.SetUnlockStatus(true);
                _errorLabel.text = string.Empty;
            }
            else
            {
                _newGameButtonCanvasGroup.SetUnlockStatus(false);
                _errorLabel.text = "Pack is not set";
            }
            
            _packNameLabel.text = packDirectory;
        }
    }
}