using Microsoft.EntityFrameworkCore;

namespace J4JSoftware.BlazorEmailBased;

public class SiteSettings
{
    public const string SectionName = "SiteSettings";
    public const int DefaultItemsPerPage = 5;
    public const int DefaultLoginLinkLifetimeMinutes = 15;

    private string[] _adminEmails = [];

    public void OnSettingsChanged(string[] adminEmails) 
    { 
        _adminEmails = adminEmails;
        UpdateAdminImpersonation();
    }

    public string SiteName { get; set; } = string.Empty;
    public int ItemsPerPage { get; set; } = DefaultItemsPerPage;

    public int LoginLinkLifetimeMinutes
    {
        get => field;
        set => field = value < 1 ? DefaultLoginLinkLifetimeMinutes : value;
    }

    public bool RestrictNonAdmins { get; set; } = false;
    
    public RouteEmailsToAdmin AdminImpersonation { get; private set; } = RouteEmailsToAdmin.Offline;

    private void UpdateAdminImpersonation()
    {
        if (!RouteLoginsToAdminEmail)
        {
            AdminImpersonation = RouteEmailsToAdmin.Offline;
            return;
        }

        if(string.IsNullOrWhiteSpace(PreferredAdminEmail))
        {
            AdminImpersonation = RouteEmailsToAdmin.AdminEmailUndefined;
            return;
        }

        if(!_adminEmails.Any())
        {
            AdminImpersonation = RouteEmailsToAdmin.NoAdminEmailsDefined;
            return;
        }

        if( !_adminEmails.Contains(PreferredAdminEmail) )
        {
            AdminImpersonation = RouteEmailsToAdmin.AdminEmailNotFound;
            return;
        }

        AdminImpersonation = RouteEmailsToAdmin.Online;
    }

    public bool RouteLoginsToAdminEmail 
    {
        get => field;

        set
        {
            field = value;
            UpdateAdminImpersonation();
        }
    } = false;

    public string PreferredAdminEmail {
        get => field;

        set
        {
            field = value;
            UpdateAdminImpersonation();
        }
    } = string.Empty;

    public bool MaintenanceModeEnabled { get; set; }

    public string DefaultConnection { get; set; } = string.Empty;
}
