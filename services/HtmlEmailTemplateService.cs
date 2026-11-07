using BlazorEmailSiteCommon.emails;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace BlazorEmailSiteCommon.Services;

public interface IHtmlEmailTemplateService
{
    Task<string> GenerateLoginEmailAsync(string name, bool adminImpersonating, LoginTokenInfo tokenInfo);
    Task<string> GenerateLockedOutEmailAsync(string name);
}

public class HtmlEmailTemplateService( IServiceProvider serviceProvider, ILoggerFactory loggerFactory )
    : IHtmlEmailTemplateService
{
    public async Task<string> GenerateLoginEmailAsync( string name, bool adminImpersonating, LoginTokenInfo tokenInfo )
    {
        var parameters = ParameterView.FromDictionary( new Dictionary<string, object?>
        {
            { "Name", name },
            { "AdminImpersonating", adminImpersonating },
            { "VerificationLink", tokenInfo.VerifyUrl },
            { "LockOutLink", tokenInfo.LockoutUrl }
        } );

        return await GenerateEmailInternalAsync( typeof( LoginEmail ), parameters );
    }

    public async Task<string> GenerateLockedOutEmailAsync(string name )
    {
        var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            { "Name", name }
        });

        return await GenerateEmailInternalAsync(typeof(LockedOutEmail), parameters);
    }

    protected async Task<string> GenerateEmailInternalAsync( Type templateType, ParameterView parameters )
    {
        await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync(templateType, parameters);
            return output.ToHtmlString();
        });

        return html;
    }
}
