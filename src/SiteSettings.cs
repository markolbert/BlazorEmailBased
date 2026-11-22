#region copyright

// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// SiteSettings.cs
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

namespace J4JSoftware.BlazorEmailBased;

public class SiteSettings
{
    public const string SectionName = "SiteSettings";
    public const int DefaultItemsPerPage = 5;
    public const int DefaultLoginLinkLifetimeMinutes = 15;

    private string[] _adminEmails = [];

    public void OnSettingsChanged( string[] adminEmails )
    {
        _adminEmails = adminEmails;
        UpdateAdminImpersonation();
    }

    public string SiteName { get; set; } = string.Empty;
    public int ItemsPerPage { get; set; } = DefaultItemsPerPage;

    public int LoginLinkLifetimeMinutes
    {
        get;
        set => field = value < 1 ? DefaultLoginLinkLifetimeMinutes : value;
    }

    public bool RestrictNonAdmins { get; set; } = false;

    public RouteEmailsToAdmin AdminImpersonation { get; private set; } = RouteEmailsToAdmin.Offline;

    private void UpdateAdminImpersonation()
    {
        if( !RouteLoginsToAdminEmail )
        {
            AdminImpersonation = RouteEmailsToAdmin.Offline;
            return;
        }

        if( string.IsNullOrWhiteSpace( PreferredAdminEmail ) )
        {
            AdminImpersonation = RouteEmailsToAdmin.AdminEmailUndefined;
            return;
        }

        if( !_adminEmails.Any() )
        {
            AdminImpersonation = RouteEmailsToAdmin.NoAdminEmailsDefined;
            return;
        }

        if( !_adminEmails.Contains( PreferredAdminEmail ) )
        {
            AdminImpersonation = RouteEmailsToAdmin.AdminEmailNotFound;
            return;
        }

        AdminImpersonation = RouteEmailsToAdmin.Online;
    }

    public bool RouteLoginsToAdminEmail
    {
        get;

        set
        {
            field = value;
            UpdateAdminImpersonation();
        }
    } = false;

    public string PreferredAdminEmail
    {
        get;

        set
        {
            field = value;
            UpdateAdminImpersonation();
        }
    } = string.Empty;

    public bool MaintenanceModeEnabled { get; set; }

    public string DefaultConnection { get; set; } = string.Empty;
}
