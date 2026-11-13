using Microsoft.AspNetCore.Identity;

namespace J4JSoftware.BlazorEmailBased.Services;

public class PasswordlessLoginTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public const string ProviderName = "PasswordlessLoginTokenProvider";

    public PasswordlessLoginTokenProviderOptions()
    {
        Name = ProviderName;
        TokenLifespan = TimeSpan.FromMinutes( 15 ); // Short lifespan for security
    }
}
