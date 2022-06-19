using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using SFB;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace PartyQuiz.Constructor
{
    internal sealed class ImageDetailsPanel : UIElement
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _icon;

        [SerializeField] private Button _imageButton;
        [SerializeField] private Button _deleteButton;

        private Action<Media<Sprite>> _onImageSet;
        private Sprite _sprite;

        internal void Show([CanBeNull] Media<Sprite> sprite, Action<Media<Sprite>> onImageSet)
        {
            _onImageSet = onImageSet;

            _imageButton.onClick.AddListener(OnImageButtonPressedHandler);
            DC.AddDisposable(() => _imageButton.onClick.RemoveListener(OnImageButtonPressedHandler));

            _deleteButton.onClick.AddListener(OnDeleteButtonPressedHandler);
            DC.AddDisposable(() => _deleteButton.onClick.RemoveListener(OnDeleteButtonPressedHandler));

            if (sprite != null && sprite.Source != null)
            {
                _image.sprite = sprite.Source;
                _sprite = sprite.Source;
            }
            else
            {
                _image.sprite = null;
                _sprite = null;
            }
            
            ValidateIcons();
            ShowGameObject();
        }

        private async void OnImageButtonPressedHandler()
        {
            var result = await LoadImage();
            SetSprite(result);
        }

        private void OnDeleteButtonPressedHandler()
        {
            SetSprite(new Media<Sprite>(null, string.Empty));
        }

        private static async UniTask<Media<Sprite>> LoadImage()
        {
            var extensions = new ExtensionFilter(string.Empty, "png", "jpg", "jpeg", "bmp");

            var paths = StandaloneFileBrowser
                .OpenFilePanel("Open image", string.Empty, new[] { extensions }, false);

            if (paths.Length == 0)
                return null;

            var path = paths[0];

            var request = UnityWebRequestTexture.GetTexture("file://" + path).SendWebRequest();
            await request;

            if (request.webRequest.result != UnityWebRequest.Result.Success)
                Debug.LogError("Error load texture : " + request.webRequest.error);

            var texture = DownloadHandlerTexture.GetContent(request.webRequest);

            var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);

            var tempPath = ConstructorUtils.GetTempPath(path);
            return new Media<Sprite>(sprite, tempPath);
        }

        private void SetSprite([CanBeNull] Media<Sprite> sprite)
        {
            _image.sprite = sprite?.Source;
            _sprite = sprite?.Source;
            _onImageSet?.Invoke(sprite);

            ValidateIcons();
        }

        private void ValidateIcons()
        {
            _image.gameObject.SetActive(_sprite != null);
            _deleteButton.gameObject.SetActive(_sprite != null);
            _icon.gameObject.SetActive(_sprite == null);
        }
    }
}