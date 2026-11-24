#region copyright
// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// LoginTokenService.cs
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

using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace J4JSoftware.BlazorEmailBased.Services;

public interface ILoginTokenService<in TUser>
    where TUser : IdentityUser<int>
{
    Task<LoginTokenInfo> GetTokenInfoAsync( TUser user );
}

public abstract class LoginTokenService
{
    public const string VerifyMagicLinkPath = "verify-magic-link";
    public const string LockoutLinkPath = "lockout-link";
}

public class LoginTokenService<TUser> : LoginTokenService, ILoginTokenService<TUser>, IDisposable
    where TUser : IdentityUser<int>
{
    private readonly UserManager<TUser> _userMgr;
    private readonly NavigationManager _navMgr;
    private readonly IDisposable? _changeSubscription;

    private int _lifetime;

    public LoginTokenService(
        UserManager<TUser> userMgr,
        NavigationManager navMgr,
        IOptionsMonitor<SiteSettings> optionsMonitor
    )
    {
        _userMgr = userMgr;
        _navMgr = navMgr;

        _lifetime = optionsMonitor.CurrentValue.LoginLinkLifetimeMinutes;

        _changeSubscription = optionsMonitor.OnChange( x => { _lifetime = x.LoginLinkLifetimeMinutes; } );
    }

    public async Task<LoginTokenInfo> GetTokenInfoAsync( TUser user )
    {
        var rawToken = await _userMgr.GenerateUserTokenAsync( user,
                                                              PasswordlessLoginTokenProvider<TUser>.ProviderName,
                                                              PasswordlessLoginTokenProvider<TUser>.Purpose );

        var token = WebEncoders.Base64UrlEncode( Encoding.UTF8.GetBytes( rawToken ) );

        var now = DateTime.UtcNow;

        var loginToken = new LoginToken
        {
            Token = token,
            CreatedUtc = DateOnly.FromDateTime( now ),
            ExpiresUtc = DateOnly.FromDateTime( now.AddMinutes( _lifetime ) ),
            OriginalEmail = user.Email!
        };

        var verifyUrl = _navMgr.GetUriWithQueryParameters( $"{_navMgr.BaseUri}{VerifyMagicLinkPath}",
                                                           new Dictionary<string, object?> { { "token", token } } );

        var lockoutUrl = _navMgr.GetUriWithQueryParameters( $"{_navMgr.BaseUri}{LockoutLinkPath}",
                                                            new Dictionary<string, object?> { { "token", token } } );

        return new LoginTokenInfo( loginToken, verifyUrl, lockoutUrl );
    }

    public void Dispose()
    {
        _userMgr.Dispose();
        _changeSubscription?.Dispose();
    }
}
