using Microsoft.AspNetCore.Identity;

namespace BlazorEmailSiteCommon.Services;

public class PasswordlessLoginTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public PasswordlessLoginTokenProviderOptions()
    {
        Name = "PasswordlessLoginTokenProvider";
        TokenLifespan = TimeSpan.FromMinutes( 15 ); // Short lifespan for security
    }
}
