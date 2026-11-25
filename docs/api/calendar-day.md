# CalendarDay

`CalendarDay` contains information about a particular
selected day, as well as properties that can be set to define how
it displays:

```csharp
public class CalendarDay
{
    internal CalendarDay(
        DateOnly date
    )
    {
        Date = date;
        FirstOfMonth = date.AddDays( -date.Day + 1 );
        InCurrentMonth = date.Month == FirstOfMonth.Month && date.Year == FirstOfMonth.Year;
        InThePast = date < DateOnly.FromDateTime( DateTime.Today );
    }

    public DateOnly Date { get; }
    public DateOnly FirstOfMonth { get; }

    public bool InCurrentMonth { get; }
    public bool InThePast { get; }

    public DayState State { get; set; } = DayState.None;
    public string? BackgroundColor { get; set; }
    public List<string>? BlockColors { get; set; }
}
```

`State` is a flag enum which controls whether or not the day cell can
be clicked, and whether it is inactive for some reason (e.g., because
it's not a day within the currently displayed month). See
[DayState](day-state.md) for details.
