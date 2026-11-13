namespace J4JSoftware.BlazorEmailBased.Components;

public class CalendarDay
{
    internal static CalendarDay Create( DateOnly date, ref DateTime firstOfMonth ) => new( date, ref firstOfMonth );

    private readonly DateTime _firstOfMonth;

    private CalendarDay(
        DateOnly date,
        ref DateTime firstOfMonth
    )
    {
        _firstOfMonth = firstOfMonth;
        Date = date;
    }

    public DateOnly Date { get; }
    public bool InCurrentMonth => Date.Month == _firstOfMonth.Month && Date.Year == _firstOfMonth.Year;
    public bool InThePast => Date < DateOnly.FromDateTime( DateTime.Today );

    public DayState State { get; set; } = DayState.None;
    public string? BackgroundColor { get; set; }
    public List<string>? BlockColors { get; set; }
}
