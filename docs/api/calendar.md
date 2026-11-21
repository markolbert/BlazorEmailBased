# MyCalendar Component

`MyCalendar.razor` defines a *Razor component* which displays a simple
calendar that can be used to select dates.

Note that, as implemented, there is no notion of "what times of
which days are you unavailble?" That's because my use case was
for events that always occur at the same time.

It's called `MyCalendar` to avoid name collisions.

## Features

- Clicking a day toggles whether it is selected
- (optional) Clicking a day of the week column header selects that
day of the week in perpetuity. However, users can click a specific
day of the week to unselect it

## How It Works

The calendar component uses the Blazor `Parameter` system to communicate
with whatever page it is embedded in.

There is one property parameter, which controls whether or not days
can be selected en masse based on the day of the week:

```csharp
[ Parameter ]
public bool DaysOfWeekClickable { get; set; }
```

### Callbacks

The remaining parameters are `EventCallback` parameters which are raised
to tell the page on which the calendar is embedded that something has
happened, and to request information about how the calendar should be
displayed on update.

#### UpdateCalendarInfo

This callback:

```csharp
[ Parameter ]
public EventCallback<CalendarArgs> UpdateCalendarInfo { get; set; }
```

is raised by the calendar in the following situations:

- the calendar is first initialized
- the calendar's `Regenerate()` method was called
- the calendar was moved to the prior month
- the calendar was moved to the next month
- a specific day was clicked
- a specific day of the week header cell ("Tuesday") was clicked

The callback gives the page on which the calendar is embedded a chance
to define what the calendar should look like given its current state. It
does this by passing a `List<CalendarDay>` value containing entries for
every day in the calendar to be displayed to the calling page, letting it
define things like background colors for selected or unselected days.

#### Day of Week Header Cell Was Clicked

```csharp
[ Parameter ]
public EventCallback<DowClickedArgs> DayOfWeekClicked { get; set; }
```

```csharp
[ Parameter ]
public EventCallback<DayClickedArgs> DayClicked { get; set; }
```

## Usage

You embed the component in a page like this:

```html
<MyCalendar UpdateCalendarInfo="OnUpdateCalendarInfo"
            DayClicked="ToggleDay"
            @key="@_refreshToken"/>

@code {
    private readonly Guid _refreshToken = Guid.NewGuid();

    public void OnUpdateCalendarInfo()
    {
        ...
    }
    ...other code
```

Note also how `@key` is set. This is needed to identify the unique
instance of the component in the running Blazor server app.