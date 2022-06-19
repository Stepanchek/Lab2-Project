using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Constructor
{
    internal sealed class QuestionDataPanel : UIElement
    {
        [SerializeField] private QuestionDetailsPanel _questionDetailsPanel;
        [SerializeField] private AnswerDetailsPanel _answerDetailsPanel;
        
        internal void Show(Question question)
        {
            _questionDetailsPanel.Show(question);
            DC.AddDisposable(_questionDetailsPanel.Dispose);
            
            _answerDetailsPanel.Show(question.Answer);
            DC.AddDisposable(_answerDetailsPanel.Dispose);
            
            ShowGameObject();
        }
    }
}