#region copyright
// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// MapGetService.cs
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

using J4JSoftware.BlazorEmailBased.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace J4JSoftware.BlazorEmailBased;

public abstract class MapGetService
{
    public const string AdminImpersonatingUserClaimType = "AdminImpersonatingUser";
    public const string AdminEmailClaimType = "AdminEmail";
    public const string OriginalUserNameClaimType = "OriginalUserName";
    public const string OriginalUserIdClaimType = "OriginalUserId";
}

public class MapGetService<TUser, TDbContext> : MapGetService, IDisposable
    where TUser : IdentityUser<int>, IAdministrator
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly IDisposable? _changeSubscription;

    private string _adminEmail;
    private bool _routeLoginsToAdminEmail;

    public MapGetService(
        IOptionsMonitor<SiteSettings> optionsMonitor,
        IDbContextFactory<TDbContext> dbContextFactory )
    {
        _dbContext = dbContextFactory.CreateDbContext();

        _adminEmail = optionsMonitor.CurrentValue.PreferredAdminEmail;
        _routeLoginsToAdminEmail = optionsMonitor.CurrentValue.RouteLoginsToAdminEmail;

        _changeSubscription = optionsMonitor.OnChange(x =>
        {
            _adminEmail = x.PreferredAdminEmail;
            _routeLoginsToAdminEmail = x.RouteLoginsToAdminEmail;
        });
    }

    public async Task<IResult> VerifyMagicLinkAsync(
        string token,
        SignInManager<TUser> signInManager,
        UserManager<TUser> userManager
    )
    {
        var loginTokensSet = _dbContext.Set<LoginToken>();

        var loginToken = await loginTokensSet
           .FirstOrDefaultAsync( t => t.Token == token && !t.Used );

        if( loginToken == null || loginToken.ExpiresUtc < DateOnly.FromDateTime( DateTime.UtcNow ) )
            return Results.Redirect( "/Static/unauthorized.html" );

        loginToken.Used = true;
        await _dbContext.SaveChangesAsync();

        var originalUser = await GetUserAsync( loginToken.OriginalEmail, token, userManager );

        if( originalUser == null )
            return Results.BadRequest( "Invalid login request." );

        var adminUser = string.IsNullOrEmpty( _adminEmail )
            ? null
            : await userManager.FindByEmailAsync( _adminEmail );

        if( !_routeLoginsToAdminEmail || adminUser is not { IsAdministrator: true } )
        {
            // no need to create claims about admin impersonating a user
            await signInManager.SignInAsync( originalUser, isPersistent: false );
        }
        else
        {
            // user is valid admin impersonating a user, so store claims
            await signInManager.SignInWithClaimsAsync( originalUser,
                                                       isPersistent: false,
                                                       [
                                                           new Claim( AdminImpersonatingUserClaimType,
                                                                      true.ToString() ),
                                                           new Claim( AdminEmailClaimType, _adminEmail ),
                                                           new Claim( OriginalUserNameClaimType,
                                                                      originalUser.UserName! ),
                                                           new Claim( OriginalUserIdClaimType,
                                                                      originalUser.Id.ToString() )
                                                       ] );
        }

        return Results.Redirect( "/home" );
    }

    public async Task<IResult> LockoutLinkAsync(
        string token,
        UserManager<TUser> userManager,
        IHtmlEmailTemplateService templateService,
        IEmailService emailService,
        IConfiguration config,
        IDbContextFactory<TDbContext> dbContextFactory
    )
    {
        var dbContext = await dbContextFactory.CreateDbContextAsync();
        var loginTokensSet = dbContext.Set<LoginToken>();

        var loginToken = await loginTokensSet
           .FirstOrDefaultAsync( t => t.Token == token && !t.Used );

        if( loginToken == null || loginToken.ExpiresUtc < DateOnly.FromDateTime( DateTime.UtcNow ) )
            return Results.Redirect( "/Static/unauthorized.html" );

        loginToken.Used = true;
        await dbContext.SaveChangesAsync();

        var user = await GetUserAsync( loginToken.OriginalEmail, token, userManager );
        if( user == null )
            return Results.BadRequest( "Invalid login request." );

        try
        {
            var adminEmail = config.GetValue<string>("SiteSettings:AdminEmail")!;
            var siteName = config.GetValue<string>("SiteSettings:SiteName")!;

            var html = await templateService.GenerateLockedOutEmailAsync( user.Email! );

            await emailService.SendEmailAsync( adminEmail,
                                               $"{siteName} Locked Out",
                                               html,
                                               adminEmail,
                                               user.Email! );
        }

        // ReSharper disable once EmptyGeneralCatchClause
        catch( Exception )
        {
        }

        return Results.Redirect( "/Static/locked-out.html" );
    }

    private async Task<TUser?> GetUserAsync( string userId, string token, UserManager<TUser> userManager )
    {
        var user = await userManager.FindByEmailAsync( userId );
        if( user == null ) return null;

        var decodedBytes = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode( token );
        var cleanToken = System.Text.Encoding.UTF8.GetString( decodedBytes );

        // Verify token against the PasswordlessTokenProvider configuration
        var isValid = userManager.VerifyUserTokenAsync( user,
                                                        PasswordlessLoginTokenProvider<TUser>.ProviderName,
                                                        PasswordlessLoginTokenProvider<TUser>.Purpose,
                                                        cleanToken )
                                 .Result;

        return isValid ? user : null;
    }

    public void Dispose()
    {
        if( _dbContext is IDisposable dbContextDisposable )
            dbContextDisposable.Dispose();
        else
            _ = _dbContext.DisposeAsync().AsTask();

        _changeSubscription?.Dispose();
    }
}
