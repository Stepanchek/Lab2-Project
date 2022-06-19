using UniRx;
using UnityEngine;

namespace PartyQuiz.Structure.Runtime
{
    public sealed class Answer
    {
        public ReactiveProperty<string> Text = new();
        public ReactiveProperty<Media<Sprite>> Picture = new();

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Text.Value) == false)
                return true;

            return Picture.Value != null;
        }
    }
}