// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Based on https://github.com/dotnet/aspnetcore/blob/8ad057426fa6a27cd648b05684afddab9d97d3d9/src/Components/Endpoints/src/Forms/EndpointAntiforgeryStateProvider.cs

using DkpWeb.Blazor.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace DkpWeb.Services
{
    public class AustinEndpointAntiforgeryStateProvider : AustinAntiforgeryStateProvider
    {
        private readonly IAntiforgery antiforgery;
        private readonly IHttpContextAccessor accessor;

        public AustinEndpointAntiforgeryStateProvider(IAntiforgery antiforgery, PersistentComponentState state, IHttpContextAccessor accessor)
            : base(state)
        {
            this.antiforgery = antiforgery;
            this.accessor = accessor;
        }

        public override AntiforgeryRequestToken GetAntiforgeryToken()
        {
            var context = accessor.HttpContext;
            if (context == null)
            {
                return null;
            }

            // We already have a callback setup to generate the token when the response starts if needed.
            // If we need the tokens before we start streaming the response, we'll generate and store them;
            // otherwise we'll just retrieve them.
            // In case there are no tokens available, we are going to return null and no-op.
            var tokens = !context.Response.HasStarted ? antiforgery.GetAndStoreTokens(context) : antiforgery.GetTokens(context);
            if (tokens.RequestToken is null)
            {
                return null;
            }

            return new AntiforgeryRequestToken(tokens.RequestToken, tokens.FormFieldName);
        }

    }
}
