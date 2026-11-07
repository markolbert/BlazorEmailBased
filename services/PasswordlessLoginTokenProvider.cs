using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlazorEmailSiteCommon.Services;

public class PasswordlessLoginTokenProvider<TUser>(
    IDataProtectionProvider dataProtectionProvider,
    IOptions<PasswordlessLoginTokenProviderOptions> options,
    ILogger<DataProtectorTokenProvider<TUser>> logger
)
    : DataProtectorTokenProvider<TUser>( dataProtectionProvider, options, logger )
    where TUser : IdentityUser<int>
{
    public const string ProviderName = "PasswordlessLoginProvider";
}
