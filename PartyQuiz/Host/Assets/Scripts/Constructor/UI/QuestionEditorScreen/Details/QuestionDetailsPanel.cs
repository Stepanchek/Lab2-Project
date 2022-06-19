using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Constructor
{
    internal sealed class QuestionDetailsPanel : UIElement
    {
        [SerializeField] private TextDetailsPanel _textDetailsPanel;
        [SerializeField] private ImageDetailsPanel _imageDetailsPanel;
        [SerializeField] private AudioDetailsPanel _audioDetailsPanel;

        internal void Show(Question question)
        {
            _textDetailsPanel.Show(question.Text.Value, text => question.Text.Value = text);
            DC.AddDisposable(_textDetailsPanel.Dispose);
            
            _imageDetailsPanel.Show(question.Picture.Value, image => question.Picture.Value = image);
            DC.AddDisposable(_imageDetailsPanel.Dispose);
            
            _audioDetailsPanel.Show(question.Audio.Value, clip => question.Audio.Value = clip);
            DC.AddDisposable(_audioDetailsPanel.Dispose);
            
            ShowGameObject();
        }
    }
}