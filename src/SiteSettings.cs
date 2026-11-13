namespace J4JSoftware.BlazorEmailBased;

public class SiteSettings
{
    public const string SectionName = "SiteSettings";

    public const int DefaultItemsPerPage = 5;
    public const int DefaultLoginLinkLifetimeMinutes = 15;

    public string SiteName { get; set; } = string.Empty;
    public int ItemsPerPage { get; set; } = DefaultItemsPerPage;

    public int LoginLinkLifetimeMinutes
    {
        get => field;
        set => field = value < 1 ? DefaultLoginLinkLifetimeMinutes : value;
    }

    public bool RestrictNonAdmins { get; set; } = false;
    
    public bool RouteLoginsToAdminEmail { get; set; } = false;
    public string AdminEmail { get; set; } = string.Empty;

    public bool MaintenanceModeEnabled { get; set; }

    public string DefaultConnection { get; set; } = string.Empty;
}
