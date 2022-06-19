using UniRx;
using UnityEngine;

namespace PartyQuiz.Structure.Runtime
{
    public enum EQuestionType
    {
        Normal = 0,
        CatInPoke = 1,
        Auction = 2,
    }
    
    public sealed class Question
    {
        public ReactiveProperty<int> Price = new();
        public ReactiveProperty<EQuestionType> Type = new();
        public ReactiveProperty<string> Text = new();
        public ReactiveProperty<Media<Sprite>> Picture = new();
        public ReactiveProperty<Media<AudioClip>> Audio = new();
        public Answer Answer = new();

        public bool WasAnswered;

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Text.Value) == false)
                return true;

            if (Audio.Value != null)
                return true;

            return Picture.Value != null;
        }
    }
}