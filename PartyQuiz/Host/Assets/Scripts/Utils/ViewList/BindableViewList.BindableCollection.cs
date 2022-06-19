using System;
using JetBrains.Annotations;
using UnityEngine;

namespace PartyQuiz.Utils
{
    public sealed partial class BindableViewList<TItem, TView>
    {
        public BindableViewList(
            BindableCollection<TItem> items,
            Func<TItem, TView> template,
            Func<TItem, Transform> container,
            Action<TItem, TView> addAction,
            [CanBeNull] Func<TItem, bool> predicate = null)
            : base(items, template, container, addAction, predicate)
        {
            Subscribe(items);
        }

        public BindableViewList(
            BindableCollection<TItem> items,
            Func<TItem, (TView, Transform)> data,
            Action<TItem, TView> addAction,
            [CanBeNull] Func<TItem, bool> predicate = null)
            : base(items, func => data(func).Item1, func => data(func).Item2, addAction, predicate)
        {
            Subscribe(items);
        }

        public BindableViewList(
            BindableCollection<TItem> items,
            TView template,
            Func<TItem, Transform> container,
            Action<TItem, TView> addAction,
            [CanBeNull] Func<TItem, bool> predicate = null)
            : this(items, _ => template, container, addAction, predicate)
        {
        }

        public BindableViewList(
            BindableCollection<TItem> items,
            Func<TItem, TView> template,
            Transform container,
            Action<TItem, TView> addAction,
            [CanBeNull] Func<TItem, bool> predicate = null)
            : this(items, template, _ => container, addAction, predicate)
        {
        }

        public BindableViewList(
            BindableCollection<TItem> items,
            TView template,
            Transform container,
            Action<TItem, TView> addAction,
            [CanBeNull] Func<TItem, bool> predicate = null)
            : this(items, _ => template, _ => container, addAction, predicate)
        {
        }
    }
}