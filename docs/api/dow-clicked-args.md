# DowClickedArgs

Used to pass information from the `MyCalendar` component to the page
using the component when a day of the week header cell is clicked.

```csharp
public record DowClickedArgs( DayOfWeek DayOfWeek, List<CalendarDay> DaysDisplayed );
```

`DaysDisplayed` contains the corresponding days of the week in the
currently displayed month. These entities can be updated by the host
page to change how they look.
