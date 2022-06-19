using System;
using TMPro;
using UnityEngine.EventSystems;

namespace PartyQuiz.Utils
{
    public sealed class TMPInputFieldNoScroll : TMP_InputField
    {
        private Action<PointerEventData> _onScroll;

        public void Show(Action<PointerEventData> onScroll)
        {
            _onScroll = onScroll;
        }

        public override void OnScroll(PointerEventData eventData)
        {
            _onScroll?.Invoke(eventData);
        }
    }
}