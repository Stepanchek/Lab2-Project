namespace PartyQuiz.Utils
{
    public sealed partial class BindableViewList<TItem, TView> : ViewList<TItem, TView>
        where TView : DisposableObject
    {
        private BindableCollection<TItem> _items = new();

        private void Subscribe(BindableCollection<TItem> items)
        {
            _items = items;

            _items.OnItemAdded += OnItemAddedHandler;
            _items.OnClear += OnClearHandler;
            _items.OnItemRemoved += OnItemRemovedHandler;
        }

        private void OnItemAddedHandler(TItem item)
        {
            if (Contains(item))
                return;

            Add(item);
        }

        private void OnClearHandler()
        {
            Clear();
        }

        private void OnItemRemovedHandler(TItem item)
        {
            if (Contains(item) == false)
                return;

            Remove(item);
        }

        public override void Dispose()
        {
            _items.OnItemAdded -= OnItemAddedHandler;
            _items.OnClear -= OnClearHandler;
            _items.OnItemRemoved -= OnItemRemovedHandler;

            base.Dispose();
        }
    }
}