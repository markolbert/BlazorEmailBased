# CalendarDay

`CalendarDay` contains information about a particular
selected day, as well as properties that can be set to define how
it displays:

```csharp
// internal details omitted (see source code for complete definition)
public class CalendarDay
{
    public DateOnly Date { get; }

    // indicates whether the day is part of the current
    // month being displayed by the component
    public bool InCurrentMonth { get;}

    // indicates whether the day is before today's date
    public bool InThePast { get; }

    // indicates whether the day is clickable, inactive, both
    // or neither
    public DayState State { get; set; }

    // the background color (hex code) of the selected day 
    // in the component
    public string? BackgroundColor { get; set; }

    // a list of colors (hex codes) to be displayed in day's
    // display cell
    public List<string>? BlockColors { get; set; }
}
```