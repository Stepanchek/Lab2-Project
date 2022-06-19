using System.Linq;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Constructor
{
    internal sealed class RoundsListPanel : UIElement
    {
        [SerializeField] private ThemesListPanel _themesListPanel;

        [SerializeField] private RoundView _roundViewTemplate;
        [SerializeField] private RectTransform _container;

        [SerializeField] private NewListView _newRoundView;
        [SerializeField] private MessageWindow _confirmationWindow;

        private BindableViewList<Round, RoundView> _viewList;
        private Pack _pack;

        internal void Show(Pack pack)
        {
            _pack = pack;

            DC.Dispose();

            _viewList = new BindableViewList<Round, RoundView>(
                _pack.Rounds,
                _roundViewTemplate,
                _container,
                (item, view) =>
                {
                    var index = _pack.Rounds.IndexOf(item);
                    view.Show(item, index, SelectRound, OnDeleteButtonPressedHandler);

                    _newRoundView.SetAsLast();
                });

            DC.AddDisposable(_viewList);

            _newRoundView.Show(OnNewRoundCreatedHandler);
            _newRoundView.SetAsLast();

            TryCreateAndSelectNewRound();
            ShowGameObject();
        }

        private void SelectRound(Round round)
        {
            _themesListPanel.Show(round, _pack.Rounds.IndexOf(round));

            foreach (var (existingRound, existingView) in _viewList)
                existingView.SetSelectedState(existingRound == round);
        }

        private void OnDeleteButtonPressedHandler(Round round)
        {
            _confirmationWindow.Show(null, () => _pack.Rounds.Remove(round));
        }

        private void TryCreateAndSelectNewRound()
        {
            if (_pack.Rounds.Count <= 0)
            {
                var newRound = new Round();

                AddRound(newRound);
                SelectRound(newRound);
            }
            else
            {
                SelectRound(_pack.Rounds.First());
            }
        }

        private void AddRound(Round round)
        {
            _pack.Rounds.Add(round);
        }

        private void OnNewRoundCreatedHandler()
        {
            AddRound(new Round());
        }
    }
}