namespace J4JSoftware.BlazorEmailBased.Components;

[ Flags ]
public enum DayState
{
    Clickable = 1 << 0,
    Inactive = 1 << 1,

    None = 0
}
