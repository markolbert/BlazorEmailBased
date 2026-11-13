using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BlazorEmailSiteCommon.Middleware;

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
