using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;

namespace PartyQuiz.Constructor
{
    internal sealed class PackNamePanel : UIElement
    {
        [SerializeField] private TMP_InputField _packNameInputField;
        
        private Pack _pack;

        internal void Show(Pack pack)
        {
            _pack = pack;
            _packNameInputField.text = _pack.Name.Value;
            
            _packNameInputField.onEndEdit.AddListener(OnPackNameEndEditHandler);
            DC.AddDisposable(() => _packNameInputField.onEndEdit.RemoveListener(OnPackNameEndEditHandler));
        }

        private void OnPackNameEndEditHandler(string value)
        {
            _pack.Name.Value = value;
        }
    }
}