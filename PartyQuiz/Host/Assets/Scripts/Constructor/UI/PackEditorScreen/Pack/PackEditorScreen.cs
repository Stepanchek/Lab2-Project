using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Constructor
{
    internal sealed class PackEditorScreen : UIElement
    {
        [SerializeField] private PackControlPanel _packControlPanel;
        [SerializeField] private RoundsListPanel _roundsListPanel;

        internal void Show(Pack pack)
        {
            Clear();

            _packControlPanel.Show(pack);
            _roundsListPanel.Show(pack);
            
            ShowGameObject();
        }

        private void Clear()
        {
            _roundsListPanel.Dispose();
        }
    }
}