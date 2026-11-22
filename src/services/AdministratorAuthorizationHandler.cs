#region copyright

// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// AdministratorAuthorizationHandler.cs
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
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace J4JSoftware.BlazorEmailBased.Services;

public sealed class AdministratorAuthorizationHandler<TUser> : AuthorizationHandler<AdministratorRequirement>,
    IDisposable
    where TUser : IdentityUser<int>, IAdministrator
{
    private readonly UserManager<TUser> _userManager;
    private readonly IDisposable? _changeSubscription;

    private bool _routeLoginsToAdminEmail;

    public AdministratorAuthorizationHandler(
        UserManager<TUser> userManager,
        IOptionsMonitor<SiteSettings> optionsMonitor
    )
    {
        _userManager = userManager;

        _routeLoginsToAdminEmail = optionsMonitor.CurrentValue.RouteLoginsToAdminEmail;

        _changeSubscription = optionsMonitor.OnChange( x => { _routeLoginsToAdminEmail = x.RouteLoginsToAdminEmail; } );
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdministratorRequirement requirement
    )
    {
        if( context.User.Identity?.IsAuthenticated != true )
            return;

        var member = await _userManager.GetUserAsync( context.User );

        if( member?.IsAdministrator == true && !_routeLoginsToAdminEmail )
            context.Succeed( requirement );
    }

    public void Dispose()
    {
        _userManager.Dispose();
        _changeSubscription?.Dispose();
    }
}
