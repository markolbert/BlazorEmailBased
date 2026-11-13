using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;

namespace J4JSoftware.BlazorEmailBased.Services;

public interface ILoginTokenService<in TUser>
    where TUser : IdentityUser<int>
{
    Task<LoginTokenInfo> GetTokenInfoAsync( TUser user );
}

public abstract class LoginTokenService
{
    public const string VerifyMagicLinkPath = "verify-magic-link";
    public const string LockoutLinkPath = "lockout-link";
}

public class LoginTokenService<TUser> : LoginTokenService, ILoginTokenService<TUser>, IDisposable
    where TUser : IdentityUser<int>
{
    private readonly UserManager<TUser> _userMgr;
    private readonly NavigationManager _navMgr;
    private readonly IDisposable? _changeSubscription;

    private int _lifetime;

    public LoginTokenService(
        UserManager<TUser> userMgr,
        NavigationManager navMgr,
        IOptionsMonitor<SiteSettings> optionsMonitor
        )
    {
        _userMgr = userMgr;
        _navMgr = navMgr;

        _lifetime = optionsMonitor.CurrentValue.LoginLinkLifetimeMinutes;

        _changeSubscription = optionsMonitor.OnChange(x =>
        {
            _lifetime = x.LoginLinkLifetimeMinutes;
        });
    }

    public async Task<LoginTokenInfo> GetTokenInfoAsync( TUser user )
    {
        var rawToken = await _userMgr.GenerateUserTokenAsync( user,
                                                              PasswordlessLoginTokenProvider<TUser>.ProviderName,
                                                              PasswordlessLoginTokenProvider<TUser>.Purpose );

        var token = WebEncoders.Base64UrlEncode( Encoding.UTF8.GetBytes( rawToken ) );

        var now = DateTime.UtcNow;

        var loginToken = new LoginToken
        {
            Token = token,
            CreatedUtc = DateOnly.FromDateTime( now ),
            ExpiresUtc = DateOnly.FromDateTime( now.AddMinutes( _lifetime ) ),
            OriginalEmail = user.Email!
        };

        var verifyUrl = _navMgr.GetUriWithQueryParameters( $"{_navMgr.BaseUri}{VerifyMagicLinkPath}",
                                                          new Dictionary<string, object?> { { "token", token } } );

        var lockoutUrl = _navMgr.GetUriWithQueryParameters( $"{_navMgr.BaseUri}{LockoutLinkPath}",
                                                           new Dictionary<string, object?> { { "token", token } } );

        return new LoginTokenInfo( loginToken, verifyUrl, lockoutUrl );
    }

    public void Dispose()
    {
        _userMgr.Dispose();
        _changeSubscription?.Dispose();
    }
}
