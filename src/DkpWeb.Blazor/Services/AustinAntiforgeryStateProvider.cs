﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Based on https://github.com/dotnet/aspnetcore/blob/8ad057426fa6a27cd648b05684afddab9d97d3d9/src/Components/Shared/src/DefaultAntiforgeryStateProvider.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;

namespace DkpWeb.Blazor.Services
{
    public class AustinAntiforgeryStateProvider : AntiforgeryStateProvider, IDisposable
    {
        private const string PersistenceKey = $"__austin__{nameof(AntiforgeryRequestToken)}";
        private readonly PersistingComponentStateSubscription _subscription;
        private readonly AntiforgeryRequestToken? _currentToken;

        [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = $"{nameof(AustinAntiforgeryStateProvider)} uses the {nameof(PersistentComponentState)} APIs to deserialize the token, which are already annotated.")]
        public AustinAntiforgeryStateProvider(PersistentComponentState state)
        {
            // Automatically flow the Request token to server/wasm through
            // persistent component state. This guarantees that the antiforgery
            // token is available on the interactive components, even when they
            // don't have access to the request.
            _subscription = state.RegisterOnPersisting(() =>
            {
                state.PersistAsJson(PersistenceKey, GetAntiforgeryToken());
                return Task.CompletedTask;
            });

            state.TryTakeFromJson(PersistenceKey, out _currentToken);
        }

        /// <inheritdoc />
        public override AntiforgeryRequestToken? GetAntiforgeryToken() => _currentToken;

        /// <inheritdoc />
        public void Dispose() => _subscription.Dispose();
    }
}
