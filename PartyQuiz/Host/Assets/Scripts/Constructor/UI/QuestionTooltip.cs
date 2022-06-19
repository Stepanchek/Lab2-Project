using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PartyQuiz.Constructor
{
    internal sealed class QuestionTooltip : UIElement
    {
        [SerializeField] private Image _questionCheckmark;
        [SerializeField] private Image _answerCheckmark;

        [SerializeField] private Sprite _goodCheckmark;
        [SerializeField] private Sprite _badCheckmark;

        [SerializeField] private Color _goodIconColor;
        [SerializeField] private Color _badIconColor;

        [SerializeField] private TextMeshProUGUI _questionTextLabel;
        [SerializeField] private TextMeshProUGUI _questionPictureLabel;
        [SerializeField] private TextMeshProUGUI _questionAudioLabel;

        [SerializeField] private TextMeshProUGUI _answerTextLabel;
        [SerializeField] private TextMeshProUGUI _answerPictureLabel;

        [SerializeField] private Color _goodTextColor;
        [SerializeField] private Color _badTextColor;
        
        internal void Show(QuestionView view, Question question)
        {
            _questionCheckmark.sprite = question.IsValid() ? _goodCheckmark : _badCheckmark;
            _answerCheckmark.sprite = question.Answer.IsValid() ? _goodCheckmark : _badCheckmark;

            _questionCheckmark.color = question.IsValid() ? _goodIconColor : _badIconColor;
            _answerCheckmark.color = question.Answer.IsValid() ? _goodIconColor : _badIconColor;
            
            _questionTextLabel.color = string.IsNullOrEmpty(question.Text.Value) ? _badTextColor : _goodTextColor;
            _questionPictureLabel.color = question.Picture.Value == null ? _badTextColor : _goodTextColor;
            _questionAudioLabel.color = question.Audio.Value == null ? _badTextColor : _goodTextColor;
            
            _answerTextLabel.color = string.IsNullOrEmpty(question.Answer.Text.Value) ? _badTextColor : _goodTextColor;
            _answerPictureLabel.color = question.Answer.Picture.Value == null ? _badTextColor : _goodTextColor;
            
            transform.position = view.transform.position + new Vector3(210, 0, 0);
            
            ShowGameObject();
        }
    }
}