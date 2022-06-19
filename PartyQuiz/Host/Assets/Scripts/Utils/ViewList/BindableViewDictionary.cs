using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace PartyQuiz.Utils
{
    public sealed class BindableViewDictionary<T1, T2, TView> : ViewList<KeyValuePair<T1, T2>, TView>
        where TView : DisposableObject
    {
        private BindableDictionary<T1, T2> _items;

        public BindableViewDictionary(
            BindableDictionary<T1, T2> items,
            TView template,
            Transform container,
            Action<KeyValuePair<T1, T2>, TView> addAction,
            [CanBeNull] Func<KeyValuePair<T1, T2>, bool> predicate = null)
            : base(items, _ => template, _ => container, addAction, predicate)
        {
            Subscribe(items);
        }

        private void Subscribe(BindableDictionary<T1, T2> items)
        {
            _items = items;

            _items.OnItemAdded += OnItemAddedHandler;
            _items.OnClear += OnClearHandler;
            _items.OnItemRemoved += OnItemRemovedHandler;
        }

        private void OnItemAddedHandler(KeyValuePair<T1, T2> keyValuePair)
        {
            Add(keyValuePair);
        }

        private void OnClearHandler()
        {
            Clear();
        }

        private void OnItemRemovedHandler(KeyValuePair<T1, T2> keyValuePair)
        {
            Remove(keyValuePair);
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