#region copyright
// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// ConditionalAdminHandler.cs
//
// This file is part of JumpForJoy Software's BlazorEmailBased package.
// 
// BlazorEmailBased is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// BlazorEmailBased is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with BlazorEmailBased. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace J4JSoftware.BlazorEmailBased.Services;

public class ConditionalAdminHandler : AuthorizationHandler<ConditionalAdminRequirement>, IDisposable
{
    private readonly IDisposable? _changeSubscription;
    private bool _restrictNonAdmins;

    public ConditionalAdminHandler(
        IOptionsMonitor<SiteSettings> optionsMonitor
        )
    {
        _restrictNonAdmins = optionsMonitor.CurrentValue.RestrictNonAdmins;

        _changeSubscription = optionsMonitor.OnChange(x =>
        {
            _restrictNonAdmins = x.RestrictNonAdmins;
        });
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ConditionalAdminRequirement requirement
    )
    {
        if( !_restrictNonAdmins )
        {
            // flag off: everyone passes, no admin check
            context.Succeed( requirement );
            return Task.CompletedTask;
        }

        // flag on: only Administrators pass
        if( context.User.Identity?.IsAuthenticated == true && context.User.IsInRole( "Administrator" ) )
            context.Succeed( requirement );

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _changeSubscription?.Dispose();
    }
}
