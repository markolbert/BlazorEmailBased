using Microsoft.AspNetCore.Authorization;

namespace BlazorEmailSiteCommon.Services;

public class ConditionalAdminRequirement : IAuthorizationRequirement
{
    public const string PolicyName = "ConditionalAdmin";
}
