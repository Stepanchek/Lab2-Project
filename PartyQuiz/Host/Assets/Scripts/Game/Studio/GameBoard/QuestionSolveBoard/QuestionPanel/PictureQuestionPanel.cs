using Cysharp.Threading.Tasks;
using PartyQuiz.Utils;
using Redcode.Awaiting;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio.Board
{
    internal sealed class PictureQuestionPanel : ObjectWithDisposableContainer, IQuestionPanel<Sprite>
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public bool CanProceed { get; private set; } = true;

        public void Show(Sprite value)
        {
            CanProceed = false;
            
            SetSprite(value);
            ShowGameObject();

            Wait().HandleExceptions();
        }

        public void SetPaused(bool isPaused)
        {
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

        private async UniTask Wait()
        {
            await new WaitForSeconds(3.0f);

            CanProceed = true;
        }
    }
}