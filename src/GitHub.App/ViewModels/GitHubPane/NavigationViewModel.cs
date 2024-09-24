﻿using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using GitHub.Extensions;
using ReactiveUI;

namespace GitHub.ViewModels.GitHubPane
{
    /// <summary>
    /// A view model that supports back/forward navigation of an inner content page.
    /// </summary>
    [Export(typeof(INavigationViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        readonly ReactiveList<IPanePageViewModel> history;
        readonly ObservableAsPropertyHelper<IPanePageViewModel> content;
        int index = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationViewModel"/> class.
        /// </summary>
        public NavigationViewModel()
        {
            history = new ReactiveList<IPanePageViewModel>();
            history.Changing.Subscribe(CollectionChanging);
            history.Changed.Subscribe(CollectionChanged);

            var pos = this.WhenAnyValue(
                x => x.Index,
                x => x.History.Count,
                (i, c) => new { Index = i, Count = c });

            content = pos
                .Where(x => x.Index < x.Count)
                .Select(x => x.Index != -1 ? history[x.Index] : null)
                .StartWith((IPanePageViewModel)null)
                .ToProperty(this, x => x.Content);

            this.WhenAnyValue(x => x.Content)
              .Buffer(2, 1)
              .Subscribe(x => {
                  if (x[0] != null && history.Contains(x[0])) x[0].Deactivated();
                  x[1]?.Activated();
              });

            NavigateBack = ReactiveCommand.Create(() => { }, pos.Select(x => x.Index > 0));
            NavigateBack.Subscribe(_ => Back());
            NavigateForward = ReactiveCommand.Create(() => { }, pos.Select(x => x.Index < x.Count - 1));
            NavigateForward.Subscribe(_ => Forward());
        }

        /// <inheritdoc/>
        public IPanePageViewModel Content => content.Value;

        /// <inheritdoc/>
        public int Index
        {
            get { return index; }
            set { this.RaiseAndSetIfChanged(ref index, value); }
        }

        /// <inheritdoc/>
        public IReadOnlyReactiveList<IPanePageViewModel> History => history;

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> NavigateBack { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> NavigateForward { get; }

        /// <inheritdoc/>
        public void NavigateTo(IPanePageViewModel page)
        {
            Guard.ArgumentNotNull(page, nameof(page));

            history.Insert(index + 1, page);
            ++Index;

            if (index < history.Count - 1)
            {
                history.RemoveRange(index + 1, history.Count - (index + 1));
            }
        }

        /// <inheritdoc/>
        public bool Back()
        {
            if (index == 0)
                return false;
            --Index;
            return true;
        }

        /// <inheritdoc/>
        public bool Forward()
        {
            if (index >= history.Count - 1)
                return false;
            ++Index;
            return true;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            Index = -1;
            history.Clear();
        }

        public int RemoveAll(IPanePageViewModel page)
        {
            var count = 0;
            while (history.Remove(page)) ++count;
            return count;
        }
             
        void BeforeItemAdded(IPanePageViewModel page)
        {
            if (!history.Contains(page))
            {
                page.CloseRequested.Subscribe(_ => RemoveAll(page));
            }
        }

        void ItemRemoved(int removedIndex, IPanePageViewModel page)
        {
            if (removedIndex <= Index)
            {
                --Index;
            }

            if (!history.Contains(page))
            {
                if (Content == page) page.Deactivated();
                page.Dispose();
            }
        }

        void CollectionChanging(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (IPanePageViewModel page in e.NewItems)
                {
                    BeforeItemAdded(page);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var page in history)
                {
                    page.Dispose();
                }
            }
        }

        void CollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            using (DelayChangeNotifications())
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    for (var i = 0; i < e.OldItems.Count; ++i)
                    {
                        ItemRemoved(e.OldStartingIndex + i, (IPanePageViewModel)e.OldItems[i]);
                    }
                }
            }
        }
    }
}