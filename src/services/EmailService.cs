#region copyright

// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// EmailService.cs
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

using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace J4JSoftware.BlazorEmailBased.Services;

public interface IEmailService
{
    Task SendEmailAsync(
        string toAddress,
        string subject,
        string bodyHtml,
        string fromAddress,
        params string[] ccAddresses
    );
}

public class GraphEmailService( IConfiguration config ) : IEmailService
{
    public async Task SendEmailAsync(
        string toAddress,
        string subject,
        string bodyHtml,
        string fromAddress,
        params string[] ccAddresses
    )
    {
        var tenantId = config[ "AzureEmail:TenantId" ];
        var clientId = config[ "AzureEmail:ClientId" ];
        var clientSecret = config[ "AzureEmail:ClientSecret" ];

        // *** WARNING!!! This code should only run on the server side.
        // Do not expose your client secret in client-side Blazor code. ***
        var credential = new ClientSecretCredential( tenantId, clientId, clientSecret );
        var scopes = new[] { "https://graph.microsoft.com/.default" };
        var graphClient = new GraphServiceClient( credential, scopes );

        var requestBody = new SendMailPostRequestBody
        {
            Message = new Message
            {
                Subject = subject,
                Body = new ItemBody { ContentType = BodyType.Html, Content = bodyHtml },
                ToRecipients =
                [
                    new Recipient { EmailAddress = new EmailAddress { Address = toAddress } }
                ],
                CcRecipients = ccAddresses.Length == 0
                    ? []
                    :
                    [
                        .. ccAddresses.Select( cc => new Recipient
                        {
                            EmailAddress = new EmailAddress { Address = cc }
                        } )
                    ]
            },
            SaveToSentItems = true
        };

        // Note: The 'fromAddress' mailbox must grant "Send As" permissions to this app registration in Exchange admin center.
        await graphClient.Users[ fromAddress ]
                         .SendMail
                         .PostAsync( requestBody );
    }
}
