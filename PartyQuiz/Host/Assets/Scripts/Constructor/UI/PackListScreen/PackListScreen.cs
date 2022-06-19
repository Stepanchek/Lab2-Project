using System;
using JetBrains.Annotations;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Constructor
{
    internal sealed class PackListScreen : UIElement
    {
        [SerializeField] private PackView _packViewTemplate;
        [SerializeField] private RectTransform _container;

        [SerializeField] private NewListView _newPackView;

        [SerializeField] private MessageWindow _packCreationWindow;
        [SerializeField] private MessageWindow _confirmationWindow;

        private readonly BindableCollection<Pack> _packs = new();
        private PackLoader _packLoader;

        private Action<Pack> _onNewPackCreated;

        internal void Show(Action<Pack> onNewPackCreated)
        {
            DC.Dispose();

            _onNewPackCreated = onNewPackCreated;

            _packLoader = new PackLoader();

            var viewList = new BindableViewList<Pack, PackView>(
                _packs,
                _packViewTemplate,
                _container,
                (item, view) => { view.Show(item, OnDeleteButtonHandler); });

            DC.AddDisposable(viewList);
            DC.AddDisposable(() => _packs.Clear());

            _newPackView.Show(OnNewPackCreatedHandler);
            _newPackView.SetAsLast();

            ShowGameObject();
        }

        private void OnDeleteButtonHandler(Pack pack)
        {
            _confirmationWindow.Show(null, () => _packs.Remove(pack));
        }

        private void AddPack([CanBeNull] Pack pack)
        {
            if (pack == null)
                return;

            _packs.Add(pack);
            _newPackView.SetAsLast();

            _onNewPackCreated?.Invoke(pack);
        }

        private void OnNewPackCreatedHandler()
        {
            _packCreationWindow.Show(
                () => { AddPack(_packLoader.LoadPack()); },
                () => AddPack(new Pack()));
        }
    }
}