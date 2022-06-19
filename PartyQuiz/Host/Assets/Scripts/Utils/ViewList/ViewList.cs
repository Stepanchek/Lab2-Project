using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace PartyQuiz.Utils
{
    public class ViewList<TItem, TView> : IDisposable, IEnumerable<KeyValuePair<TItem, TView>>
        where TView : DisposableObject
    {
        private readonly Dictionary<TItem, TView> _views = new();

        private readonly Func<TItem, TView> _template;
        private readonly Func<TItem, Transform> _container;

        private readonly Action<TItem, TView> _addAction;

        [CanBeNull]
        private readonly Func<TItem, bool> _predicate;

        protected ViewList(IEnumerable<TItem> items,
            Func<TItem, TView> template,
            Func<TItem, Transform> container,
            Action<TItem, TView> addAction,
            [CanBeNull] Func<TItem, bool> predicate)
        {
            _template = template;
            _container = container;

            _addAction = addAction;
            _predicate = predicate;

            foreach (var item in items)
                TryCreateView(item);
        }

        protected void Add(TItem item)
        {
            TryCreateView(item);
        }

        private void TryCreateView(TItem item)
        {
            if (_predicate != null && _predicate(item) == false)
                return;

            CreateView(item);
        }

        private void CreateView(TItem item)
        {
            if (_views.ContainsKey(item))
            {
                Debug.LogError($"element {item.GetType().Name}, id: {item.GetHashCode()} is already in list. Existing items:");
                return;
            }

            var template = _template(item);
            var container = _container(item);

            var view = UnityEngine.Object.Instantiate(template, container, false);
            var castedView = view.GetComponent<TView>();

            _addAction(item, castedView);
            _views[item] = castedView;
        }

        internal bool Contains(TItem item)
        {
            return _views.ContainsKey(item);
        }

        internal void Remove(TItem item)
        {
            KillView(_views[item]);
            _views.Remove(item);
        }

        private static void KillView(TView view)
        {
            view.Dispose();
            UnityEngine.Object.DestroyImmediate(view.gameObject);
        }

        protected void Clear()
        {
            foreach (var view in _views.Values)
                KillView(view);

            _views.Clear();
        }

        public IEnumerator<KeyValuePair<TItem, TView>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TItem, TView>>)_views).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _views.GetEnumerator();
        }

        public virtual void Dispose()
        {
            Clear();
        }
    }
}