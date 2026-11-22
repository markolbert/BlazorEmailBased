#region copyright

// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// PasswordlessLoginTokenProvider.cs
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

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace J4JSoftware.BlazorEmailBased.Services;

public class PasswordlessLoginTokenProvider<TUser>(
    IDataProtectionProvider dataProtectionProvider,
    IOptions<PasswordlessLoginTokenProviderOptions> options,
    ILogger<DataProtectorTokenProvider<TUser>> logger
)
    : DataProtectorTokenProvider<TUser>( dataProtectionProvider, options, logger )
    where TUser : IdentityUser<int>
{
    public const string ProviderName = "PasswordlessLoginProvider";
    public const string Purpose = "passwordless-auth";
}
