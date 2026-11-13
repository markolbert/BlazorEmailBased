namespace J4JSoftware.BlazorEmailBased.Components;

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
