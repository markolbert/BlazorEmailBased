using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BlazorEmailSiteCommon.Services;

public interface IDeliveryEmailService<in TUser>
    where TUser : IdentityUser<int>
{
    Task<string?> GetDeliveryEmailAsync( TUser enteredUser );
}

public class DeliveryEmailService<TUser>( UserManager<TUser> userMgr, IConfiguration config )
    : IDeliveryEmailService<TUser>
    where TUser : IdentityUser<int>
{
    public async Task<string?> GetDeliveryEmailAsync( TUser enteredUser )
    {
        if( !config.GetValue<bool>( "SiteSettings:RouteLoginsToAdminEmail" ) )
            return enteredUser.Email;

        var adminEmail = config.GetValue<string>( "SiteSettings:AdminEmail" );

        if( string.IsNullOrEmpty( adminEmail ) )
            return enteredUser.Email;

        var retVal = await userMgr.FindByEmailAsync( adminEmail );

        return retVal != null && await userMgr.IsInRoleAsync( retVal, "Administrator" )
            ? retVal.Email
            : null;
    }
}
