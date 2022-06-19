using PartyQuiz.Utils;
using TMPro;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio.Board
{
    internal sealed class StartNextRoundBoard : ObjectWithDisposableContainer
    {
        [SerializeField] private TextMeshPro _nextRoundLabel;
        
        internal void Show(int currentRoundIndex)
        {
            _nextRoundLabel.text = (currentRoundIndex + 2).ToString();

            ShowGameObject();
        }
    }
}