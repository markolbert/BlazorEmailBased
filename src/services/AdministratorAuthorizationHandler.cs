using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace J4JSoftware.BlazorEmailBased.Services;

public sealed class AdministratorAuthorizationHandler<TUser> : AuthorizationHandler<AdministratorRequirement>, IDisposable
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

        _changeSubscription = optionsMonitor.OnChange( x =>
        {
            _routeLoginsToAdminEmail = x.RouteLoginsToAdminEmail;
        });
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
