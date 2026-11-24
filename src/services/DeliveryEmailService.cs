#region copyright
// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// DeliveryEmailService.cs
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

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace J4JSoftware.BlazorEmailBased.Services;

public interface IDeliveryEmailService<in TUser>
    where TUser : IdentityUser<int>
{
    Task<string?> GetDeliveryEmailAsync( TUser enteredUser );
}

public class DeliveryEmailService<TUser> : IDeliveryEmailService<TUser>, IDisposable
    where TUser : IdentityUser<int>
{
    private readonly UserManager<TUser> _userMgr;
    private readonly IDisposable? _changeSubscription;

    private bool _routeLoginsToAdminEmail;
    private string _adminEmail;

    public DeliveryEmailService(
        UserManager<TUser> userMgr,
        IOptionsMonitor<SiteSettings> optionsMonitor
    )
    {
        _userMgr = userMgr;
        _adminEmail = optionsMonitor.CurrentValue.PreferredAdminEmail;
        _routeLoginsToAdminEmail = optionsMonitor.CurrentValue.RouteLoginsToAdminEmail;

        _changeSubscription = optionsMonitor.OnChange( x =>
        {
            _routeLoginsToAdminEmail = x.RouteLoginsToAdminEmail;
            _adminEmail = x.PreferredAdminEmail;
        } );
    }

    public async Task<string?> GetDeliveryEmailAsync( TUser enteredUser )
    {
        if( !_routeLoginsToAdminEmail || string.IsNullOrEmpty( _adminEmail ) )
            return enteredUser.Email;

        var retVal = await _userMgr.FindByEmailAsync( _adminEmail );

        return retVal != null && await _userMgr.IsInRoleAsync( retVal, "Administrator" )
            ? retVal.Email
            : null;
    }

    public void Dispose()
    {
        _userMgr.Dispose();
        _changeSubscription?.Dispose();
    }
}
