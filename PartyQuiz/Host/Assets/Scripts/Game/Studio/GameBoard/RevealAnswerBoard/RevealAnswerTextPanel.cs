using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Audio;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio.Board
{
    internal sealed class RevealAnswerTextPanel : ObjectWithDisposableContainer
    {
        [SerializeField] private TextMeshPro _label;
        
        internal async UniTask Show(TextReader textReader, string answerText)
        {
            _label.text = answerText;
            
            ShowGameObject();

            await textReader.Read(answerText);
        }
    }
}