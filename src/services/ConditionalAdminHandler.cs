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
