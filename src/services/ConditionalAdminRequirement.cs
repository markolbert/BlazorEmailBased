using Microsoft.AspNetCore.Authorization;

namespace J4JSoftware.BlazorEmailBased.Services;

public class ConditionalAdminRequirement : IAuthorizationRequirement
{
    public const string PolicyName = "ConditionalAdmin";
}
