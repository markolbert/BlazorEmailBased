using Microsoft.AspNetCore.Authorization;

namespace BlazorEmailSiteCommon.Services;

public sealed class AdministratorRequirement : IAuthorizationRequirement
{
    public const string PolicyName = "Administrator";
}