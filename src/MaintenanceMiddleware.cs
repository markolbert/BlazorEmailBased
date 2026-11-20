#region copyright
// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// MaintenanceMiddleware.cs
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

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace J4JSoftware.BlazorEmailBased.Middleware;

public class MaintenanceMiddleware : IDisposable
{
    private readonly IDisposable? _changeSubscription;
    private readonly RequestDelegate _next;

    private bool _maintenanceModeEnabled;

    public MaintenanceMiddleware(
        RequestDelegate next, 
        IOptionsMonitor<SiteSettings> optionsMonitor
        )
    {
        _next = next;

        _maintenanceModeEnabled = optionsMonitor.CurrentValue.MaintenanceModeEnabled;

        _changeSubscription = optionsMonitor.OnChange(x =>
        {
            _maintenanceModeEnabled = x.MaintenanceModeEnabled;
        });
    }

    public async Task InvokeAsync( HttpContext context )
    {
        if( _maintenanceModeEnabled )
        {
            // Block SignalR connections to drop active users and prevent reconnections
            if( context.Request.Path.StartsWithSegments( "/_blazor" ) )
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return;
            }

            // Allow the static maintenance page to load
            if( context.Request.Path == "/maintenance.html" )
            {
                await _next( context );
                return;
            }

            // Redirect all other traffic to the static page
            context.Response.Redirect( "/Static/maintenance.html" );
            return;
        }

        await _next( context );
    }

    public void Dispose()
    {
        _changeSubscription?.Dispose();
    }
}
