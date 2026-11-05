using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BlazorEmailSiteCommon.Services;

public sealed class AdministratorAuthorizationHandler<TMember>(
    UserManager<TMember> userManager,
    IConfiguration config
    )
    : AuthorizationHandler<AdministratorRequirement>
where TMember : IdentityUser, IAdministrator
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdministratorRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        var member = await userManager.GetUserAsync(context.User);

        var adminIsRegular = config.GetValue<bool>( "TreatAdminAsRegular:Enabled" );

        if (member?.IsAdministrator == true && !adminIsRegular)
            context.Succeed(requirement);
    }
}
