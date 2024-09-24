﻿using System;
using GitHub.Models;
using GitHub.UI;

namespace GitHub.ViewModels.Dialog.Clone
{
    public class RepositoryItemViewModel : ViewModelBase, IRepositoryItemViewModel
    {
        public RepositoryItemViewModel(RepositoryListItemModel model, string group)
        {
            Name = model.Name;
            Owner = model.Owner;
            Icon = model.IsPrivate
                ? Octicon.@lock
                : model.IsFork
                    ? Octicon.repo_forked
                    : Octicon.repo;
            Url = model.Url;
            Group = group;
        }

        public string Caption => Owner + '/' + Name;
        public string Name { get; }
        public string Owner { get; }
        public string Group { get; }
        public Octicon Icon { get; }
        public Uri Url { get; }
    }
}
