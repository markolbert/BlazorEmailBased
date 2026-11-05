using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace BlazorEmailSiteCommon.Services;

public class ConditionalAdminHandler(IConfiguration config)
    : AuthorizationHandler<ConditionalAdminRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ConditionalAdminRequirement requirement)
    {
        var restrict = config.GetValue<bool>("SiteSettings:RestrictNonAdmins");

        if (!restrict)
        {
            // flag off: everyone passes, no admin check
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // flag on: only Administrators pass
        if (context.User.Identity?.IsAuthenticated == true &&
            context.User.IsInRole("Administrator"))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}