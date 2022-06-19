using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PartyQuiz.Constructor
{
    internal sealed class PackControlPanel : UIElement
    {
        [SerializeField] private PackNamePanel _packNamePanel;

        [SerializeField] private Button _shuffleCatsButton;
        [SerializeField] private Button _shuffleAuctionsButton;

        [SerializeField] private Button _exportButton;
        [SerializeField] private Button _deleteButton;

        private Pack _pack;

        internal void Show(Pack pack)
        {
            _pack = pack;

            _packNamePanel.Show(_pack);

            _shuffleCatsButton.onClick.AddListener(OnShuffleCatsButtonPressedHandler);
            DC.AddDisposable(() => _shuffleCatsButton.onClick.RemoveListener(OnShuffleCatsButtonPressedHandler));

            _shuffleAuctionsButton.onClick.AddListener(OnShuffleAuctionsButtonPressedHandler);
            DC.AddDisposable(() => _shuffleAuctionsButton.onClick.RemoveListener(OnShuffleAuctionsButtonPressedHandler));
            
            _exportButton.onClick.AddListener(OnExportButtonPressedHandler);
            DC.AddDisposable(() => _exportButton.onClick.RemoveListener(OnExportButtonPressedHandler));

            _deleteButton.onClick.AddListener(OnDeleteButtonPressedHandler);
            DC.AddDisposable(() => _deleteButton.onClick.RemoveListener(OnDeleteButtonPressedHandler));

            ShowGameObject();
        }

        private void OnShuffleCatsButtonPressedHandler()
        {
            _pack.AssignRandom(EQuestionType.CatInPoke, 2);
        }

        private void OnShuffleAuctionsButtonPressedHandler()
        {
            _pack.AssignRandom(EQuestionType.Auction, 2);
        }
        
        private void OnExportButtonPressedHandler()
        {
            _pack.Export();
        }

        private void OnDeleteButtonPressedHandler()
        {
            throw new System.NotImplementedException();
        }
    }
}