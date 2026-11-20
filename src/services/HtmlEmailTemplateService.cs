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

namespace J4JSoftware.BlazorEmailBased.Services;

public interface IHtmlEmailTemplateService
{
    Task<string> GenerateLoginEmailAsync(
        string name,
        string tagLine,
        string accessSlug,
        bool adminImpersonating,
        LoginTokenInfo tokenInfo
    );

    Task<string> GenerateLockedOutEmailAsync( string name );
}

public class HtmlEmailTemplateService( IServiceProvider serviceProvider, ILoggerFactory loggerFactory )
    : IHtmlEmailTemplateService
{
    public async Task<string> GenerateLoginEmailAsync(
        string name,
        string tagLine,
        string accessSlug,
        bool adminImpersonating,
        LoginTokenInfo tokenInfo
    )
    {
        var parameters = ParameterView.FromDictionary( new Dictionary<string, object?>
        {
            { "Name", name },
            { "TagLine", tagLine },
            { "AccessSlug", accessSlug },
            { "AdminImpersonating", adminImpersonating },
            { "VerificationLink", tokenInfo.VerifyUrl },
            { "LockOutLink", tokenInfo.LockoutUrl }
        } );

        return await GenerateEmailInternalAsync( typeof( LoginEmail ), parameters );
    }

    public async Task<string> GenerateLockedOutEmailAsync( string name )
    {
        var parameters = ParameterView.FromDictionary( new Dictionary<string, object?> { { "Name", name } } );

        return await GenerateEmailInternalAsync( typeof( LockedOutEmail ), parameters );
    }

    protected async Task<string> GenerateEmailInternalAsync( Type templateType, ParameterView parameters )
    {
        await using var htmlRenderer = new HtmlRenderer( serviceProvider, loggerFactory );

        var html = await htmlRenderer.Dispatcher.InvokeAsync( async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync( templateType, parameters );
            return output.ToHtmlString();
        } );

        return html;
    }
}
