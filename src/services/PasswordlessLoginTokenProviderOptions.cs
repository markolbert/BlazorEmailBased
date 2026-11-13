using Microsoft.AspNetCore.Identity;

namespace BlazorEmailSiteCommon.Services;

public class PasswordlessLoginTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public const string ProviderName = "PasswordlessLoginTokenProvider";

    public PasswordlessLoginTokenProviderOptions()
    {
        Name = ProviderName;
        TokenLifespan = TimeSpan.FromMinutes( 15 ); // Short lifespan for security
    }
}
