#region copyright

// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// SiteSettingsChangeListener.cs
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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace J4JSoftware.BlazorEmailBased;

public sealed class SiteSettingsChangeListener<TUser, TDbContext>(
    IOptionsMonitor<SiteSettings> settings,
    IDbContextFactory<TDbContext> dbContextFactory
) : IHostedService, IDisposable
    where TUser : IdentityUser<int>, IAdministrator
    where TDbContext : DbContext
{
    private IDisposable? _subscription;

    public Task StartAsync( CancellationToken cancellationToken )
    {
        var dbContext = dbContextFactory.CreateDbContext();

        DbSet<TUser>? userSet = null;

        try
        {
            userSet = dbContext.Set<TUser>();
        }
        catch
        {
            //
        }

        string[] adminEmails;

        if( userSet == null )
            adminEmails = [];
        else
        {
            adminEmails = userSet.Any( u => u.IsAdministrator && u.Email != null )
                ? [.. userSet.Where( u => u.IsAdministrator ).Select( u => u.Email! )]
                : [];
        }

        settings.CurrentValue.OnAdminEmailsChanged( adminEmails );

        _subscription = settings.OnChange( value => value.OnAdminEmailsChanged( adminEmails ) );

        return Task.CompletedTask;
    }

    public Task StopAsync( CancellationToken cancellationToken )
    {
        _subscription?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose() => _subscription?.Dispose();
}
