using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BlazorEmailSiteCommon.Middleware;

public class MaintenanceMiddleware( RequestDelegate next, IConfiguration config )
{
    public async Task InvokeAsync( HttpContext context )
    {
        // Check your config, database, or feature flag
        var isMaintenanceMode = config.GetValue<bool>( "MaintenanceMode:Enabled" );

        if( isMaintenanceMode )
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
                await next( context );
                return;
            }

            // Redirect all other traffic to the static page
            context.Response.Redirect( "/Static/maintenance.html" );
            return;
        }

        await next( context );
    }
}
