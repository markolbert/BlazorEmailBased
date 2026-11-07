using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BlazorEmailSiteCommon.Services;

public interface ILoginTokenService<in TUser>
    where TUser : IdentityUser<int>
{
    Task<LoginTokenInfo> GetTokenInfoAsync( TUser user );
}

public class LoginTokenService<TUser>(
    UserManager<TUser> userMgr, 
    NavigationManager navMgr, 
    IConfiguration config ) : ILoginTokenService<TUser>
    where TUser : IdentityUser<int>
{
    private readonly int _lifetime = config.GetValue("SiteSettings:LoginLinkLifetimeMinutes", 15);

    public async Task<LoginTokenInfo> GetTokenInfoAsync( TUser user )
    {
        var rawToken = await userMgr.GenerateUserTokenAsync(user,
                                                            "PasswordlessLoginProvider",
                                                            "passwordless-auth");

        var token = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(rawToken));

        var now = DateTime.UtcNow;

        var loginToken = new LoginToken
        {
            Token = token,
            CreatedUtc = DateOnly.FromDateTime( now ),
            ExpiresUtc = DateOnly.FromDateTime( now.AddMinutes( _lifetime ) ),
            OriginalEmail = user.Email!
        };

        var verifyUrl = navMgr.GetUriWithQueryParameters($"{navMgr.BaseUri}verify-magic-link",
                                                             new Dictionary<string, object?>
                                                             {
                                                                 { "token", token }
                                                             });

        var lockoutUrl = navMgr.GetUriWithQueryParameters($"{navMgr.BaseUri}lockout-link",
                                                              new Dictionary<string, object?>
                                                              {
                                                                  { "token", token }
                                                              });

        return new LoginTokenInfo(loginToken, verifyUrl, lockoutUrl);
    }
}