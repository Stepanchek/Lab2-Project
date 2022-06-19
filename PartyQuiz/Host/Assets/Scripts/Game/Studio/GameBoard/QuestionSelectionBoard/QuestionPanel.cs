using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class QuestionPanel : ObjectWithDisposableContainer
    {
        [SerializeField] private BoardTextCell _boardTextCell;

        internal Question Question { get; private set; }

        internal void Roll(bool value, float time)
        {
            _boardTextCell.Roll(value, time);
        }

        internal void Show(Question question)
        {
            Question = question;
            _boardTextCell.Show(Question.Price.Value.ToString());

            ShowGameObject();
        }
    }
}