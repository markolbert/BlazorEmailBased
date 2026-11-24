#region copyright
// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// HtmlEmailTemplateService.cs
//
// This file is part of JumpForJoy Software's BlazorEmailBased package.
// 
// BlazorEmailBased is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// BlazorEmailBased is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with BlazorEmailBased. If not, see <https://www.gnu.org/licenses/>.
#endregion

using J4JSoftware.BlazorEmailBased.emails;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace J4JSoftware.BlazorEmailBased.Services;

public interface IHtmlEmailTemplateService
{
    Task<string> GenerateLoginEmailAsync(
        string name,
        bool adminImpersonating,
        LoginTokenInfo tokenInfo
    );

    Task<string> GenerateLockedOutEmailAsync( string name );
}

public class HtmlEmailTemplateService : IHtmlEmailTemplateService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IDisposable? _changeSubscription;

    public HtmlEmailTemplateService(
        IOptionsMonitor<SiteSettings> optionsMonitor,
        IServiceProvider serviceProvider,
        ILoggerFactory loggerFactory
    )
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;

        SiteSettings = optionsMonitor.CurrentValue;

        _changeSubscription = optionsMonitor.OnChange( updated => { SiteSettings = updated; } );
    }

    protected SiteSettings? SiteSettings { get; private set; }

    public async Task<string> GenerateLoginEmailAsync(
        string name,
        bool adminImpersonating,
        LoginTokenInfo tokenInfo
    )
    {
        var parameters = ParameterView.FromDictionary( new Dictionary<string, object?>
        {
            { "SiteName", SiteSettings?.SiteName },
            { "TagLine", SiteSettings?.TagLine },
            { "Name", name },
            { "AdminImpersonating", adminImpersonating },
            { "VerificationLink", tokenInfo.VerifyUrl },
            { "LockOutLink", tokenInfo.LockoutUrl }
        } );

        return await GenerateEmailInternalAsync( typeof( LoginEmail ), parameters );
    }

    public async Task<string> GenerateLockedOutEmailAsync( string name )
    {
        var parameters =
            ParameterView.FromDictionary(
                new Dictionary<string, object?> { { "SiteName", SiteSettings?.SiteName }, { "Name", name } } );

        return await GenerateEmailInternalAsync( typeof( LockedOutEmail ), parameters );
    }

    protected async Task<string> GenerateEmailInternalAsync( Type templateType, ParameterView parameters )
    {
        await using var htmlRenderer = new HtmlRenderer( _serviceProvider, _loggerFactory );

        var html = await htmlRenderer.Dispatcher.InvokeAsync( async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync( templateType, parameters );
            return output.ToHtmlString();
        } );

        return html;
    }

    public void Dispose()
    {
        _changeSubscription?.Dispose();
    }
}
