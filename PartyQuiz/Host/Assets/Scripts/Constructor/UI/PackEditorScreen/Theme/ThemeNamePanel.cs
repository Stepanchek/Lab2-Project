using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;

namespace PartyQuiz.Constructor
{
    internal sealed class ThemeNamePanel : UIElement
    {
        [SerializeField] private TMP_InputField _themeNameInputField;
        
        private Theme _theme;

        internal void Show(Theme theme)
        {
            _theme = theme;
            
            _themeNameInputField.onEndEdit.AddListener(OnThemeNameChangedHandler);
            DC.AddDisposable(() => _themeNameInputField.onEndEdit.RemoveListener(OnThemeNameChangedHandler));

            _themeNameInputField.text = theme.Name.Value;
            
            ShowGameObject();
        }

        private void OnThemeNameChangedHandler(string value)
        {
            _theme.Name.Value = value;
        }
    }
}