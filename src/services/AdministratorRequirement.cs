using Microsoft.AspNetCore.Authorization;

namespace J4JSoftware.BlazorEmailBased.Services;

public sealed class AdministratorRequirement : IAuthorizationRequirement
{
    public const string PolicyName = "Administrator";
}
