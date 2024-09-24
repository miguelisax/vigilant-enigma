﻿using System;
using System.Threading.Tasks;
using GitHub.Models;

namespace GitHub.ViewModels.Dialog
{
    /// <summary>
    /// Represents the top-level view model for the GitHub dialog.
    /// </summary>
    public interface IGitHubDialogWindowViewModel : IDisposable
    {
        /// <summary>
        /// Gets the content to display in the dialog.
        /// </summary>
        IDialogContentViewModel Content { get; }

        /// <summary>
        /// Gets an observable that is signalled when when the dialog should be closed.
        /// </summary>
        /// <remarks>
        /// If the content being displayed has a return value, then this wil be returned here.
        /// </remarks>
        IObservable<object> Done { get; }

        /// <summary>
        /// Starts displaying a view model.
        /// </summary>
        /// <param name="viewModel">The view model to display.</param>
        void Start(IDialogContentViewModel viewModel);

        /// <summary>
        /// Starts displaying a view model that requires a connection.
        /// </summary>
        /// <param name="viewModel">The view model to display.</param>
        Task StartWithConnection<T>(T viewModel)
            where T : IDialogContentViewModel, IConnectionInitializedViewModel;

        /// <summary>
        /// Starts displaying a view model that requires a connection that needs to be logged out
        /// and back in.
        /// </summary>
        /// <param name="viewModel">The view model to display.</param>
        Task StartWithLogout<T>(T viewModel, IConnection connection)
            where T : IDialogContentViewModel, IConnectionInitializedViewModel;
    }
}