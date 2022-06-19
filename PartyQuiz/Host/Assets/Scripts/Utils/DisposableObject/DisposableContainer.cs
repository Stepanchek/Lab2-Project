using System;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;

namespace PartyQuiz.Utils
{
    public sealed class DisposableContainer : IDisposable
    {
        private readonly List<Action> _disposables = new();

        public T AddDisposable<T>([CanBeNull] T disposable) where T : IDisposable
        {
            if (disposable != null)
                _disposables.Add(disposable.Dispose);

            return disposable;
        }

        public void AddDisposable([CanBeNull] Action destroy)
        {
            if (destroy == null)
                return;

            _disposables.Add(destroy);
        }

        public void AddDisposable([CanBeNull] Tween tween)
        {
            if (tween == null)
                return;

            _disposables.Add(() => tween.Kill());
        }

        public void Dispose()
        {
            foreach (var destroy in _disposables)
                destroy();

            _disposables.Clear();
        }
    }
}