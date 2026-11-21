# CalendarArgs

Used to pass information from the `MyCalendar` component to the page
using the component when the selected days change. The information is
relative to the current month being displayed.

```csharp
public record CalendarArgs( DateOnly FirstOfMonth, List<CalendarDay> CalendarDays )
{
    public CalendarArgs(
        DateTime firstOfMonth,
        List<CalendarDay> calendarDays
    )
        : this( DateOnly.FromDateTime( firstOfMonth ), calendarDays )
    {
    }
}
```

Since my use case only involves days, with no time of day information,
`CalendarArgs` uses a `DateOnly` property to indicate the first day of
the month to which the information relates.

Since many libraries don't support `DateOnly`, there's a separate constructor
which accepts a `DateTime` value and converts it to a `DateOnly` value.

The rest of the record is a list of [CalendarDay](calendar-day.md)
instances, which contain information about each selected day as well as
properties that can be set by the page to adjust how each selected day
displays.
