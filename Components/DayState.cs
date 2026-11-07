namespace BlazorEmailSiteCommon.Components;

[Flags]
public enum DayState
{
    Clickable = 1 << 0,
    Inactive = 1 << 1,

    None = 0
}
