using DG.Tweening;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class BoardTextCell : ObjectWithDisposableContainer
    {
        private const float ROTATION_VALUE = 90;
        
        [SerializeField] private TextMeshPro _label;

        internal void Show(string text)
        {
            _label.text = text;

            ShowGameObject();
        }

        internal void Roll(bool value, float time)
        {
            if (value)
            {
                transform.DOLocalRotate(new Vector3(0, 0, ROTATION_VALUE), 0);
                transform.DOLocalRotate(new Vector3(0, 0, 0), time);
            }
            else
            {
                transform.DOLocalRotate(new Vector3(0, 0, 0), 0);
                transform.DOLocalRotate(new Vector3(0, 0, ROTATION_VALUE), time);
            }
        }
    }
}