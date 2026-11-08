using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BlazorEmailSiteCommon.Services;

public interface IDeliveryEmailService<in TUser>
    where TUser : IdentityUser<int>
{
    Task<string?> GetDeliveryEmailAsync( TUser enteredUser );
}

public class DeliveryEmailService<TUser> : IDeliveryEmailService<TUser>, IDisposable
    where TUser : IdentityUser<int>
{
    private readonly UserManager<TUser> _userMgr;
    private readonly IDisposable? _changeSubscription;

    private bool _routeLoginsToAdminEmail;
    private string _adminEmail;

    public DeliveryEmailService(
        UserManager<TUser> userMgr,
        IOptionsMonitor<SiteSettings> optionsMonitor
        )
    {
        _userMgr = userMgr;
        _adminEmail = optionsMonitor.CurrentValue.AdminEmail;
        _routeLoginsToAdminEmail = optionsMonitor.CurrentValue.RouteLoginsToAdminEmail;

        _changeSubscription = optionsMonitor.OnChange(x =>
        {
            _routeLoginsToAdminEmail = x.RouteLoginsToAdminEmail;
            _adminEmail = x.AdminEmail;
        });
    }

    public async Task<string?> GetDeliveryEmailAsync( TUser enteredUser )
    {
        if( !_routeLoginsToAdminEmail || string.IsNullOrEmpty( _adminEmail ) )
            return enteredUser.Email;

        var retVal = await _userMgr.FindByEmailAsync( _adminEmail );

        return retVal != null && await _userMgr.IsInRoleAsync( retVal, "Administrator" )
            ? retVal.Email
            : null;
    }

    public void Dispose()
    {
        _userMgr.Dispose();
        _changeSubscription?.Dispose();
    }
}
