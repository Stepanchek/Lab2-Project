using Cysharp.Threading.Tasks;
using PartyQuiz.Utils;
using Redcode.Awaiting;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio.Board
{
    internal sealed class RevealAnswerPicturePanel : ObjectWithDisposableContainer
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        internal async UniTask Show(Sprite sprite)
        {
            SetSprite(sprite);
            ShowGameObject();

            await new WaitForSeconds(1.5f);
        }

        private void SetSprite(Sprite sprite)
        {
            var spriteTransform = _spriteRenderer.transform;
            var size = sprite.rect.size / sprite.pixelsPerUnit;
            var frameSize = _rectTransform.sizeDelta;
            var scale = Mathf.Min(frameSize.x / size.x, frameSize.y / size.y);

            spriteTransform.localScale = Vector3.one * scale;
            spriteTransform.localPosition = frameSize / 2f - size * scale * .5f;
            _spriteRenderer.sprite = sprite;
        }
    }
}