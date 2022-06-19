using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Constructor
{
    internal sealed class AnswerDetailsPanel : UIElement
    {
        [SerializeField] private TextDetailsPanel _textDetailsPanel;
        [SerializeField] private ImageDetailsPanel _imageDetailsPanel;

        internal void Show(Answer answer)
        {
            _textDetailsPanel.Show(answer.Text.Value, text => answer.Text.Value = text);
            DC.AddDisposable(_textDetailsPanel.Dispose);
            
            _imageDetailsPanel.Show(answer.Picture.Value, image => answer.Picture.Value = image);
            DC.AddDisposable(_imageDetailsPanel.Dispose);
            
            ShowGameObject();
        }
    }
}