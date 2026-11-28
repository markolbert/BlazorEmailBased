#region copyright
// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// ILoginEmailSettings.cs
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

namespace J4JSoftware.BlazorEmailBased.Emails;

public interface ILoginEmailSettings
{
    string SiteName { get; }
    string Name { get; }
    string TagLine { get; }
    bool AdminImpersonating { get; }
    string VerificationLink { get; }
    string LockOutLink { get; }
}

public class LoginEmailSettings( SiteSettings siteSettings ) : ILoginEmailSettings
{
    public string SiteName { get; } = siteSettings.SiteName;
    public required string Name { get; set; }
    public string TagLine { get; } = siteSettings.TagLine;
    public bool AdminImpersonating { get; set; }
    public required string VerificationLink { get; set; }
    public required string LockOutLink { get; set; }
}