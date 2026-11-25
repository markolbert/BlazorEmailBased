# MyCalendar Component

`MyCalendar.razor` defines a *Razor component* which displays a simple
calendar that can be used to select dates.

Note that, as implemented, the calendar only picks *days*, not *date and
times*. That's because my use case was for events that always occur
at the same time.

It's called `MyCalendar` to avoid name collisions.

`MyCalendar` depends on CSS classes being defined. See
[calendar css](calendar-css.md) for details.

## Features

- Clicking a day toggles whether it is selected
- (optional) Clicking a day of the week column header selects that
day of the week in perpetuity. However, users can click a specific
day of the week to unselect it.

## How It Works

The calendar component uses the Blazor `Parameter` system to communicate
with whatever page (the *host page*) it is embedded in.

There is one property parameter, which controls whether or not days
can be selected en masse based on the day of the week:

```csharp
[ Parameter ]
public bool DaysOfWeekClickable { get; set; }
```

### Callbacks

The remaining parameters are `EventCallback` parameters which are raised
to tell the host page that something has happened, and to request
information about how the calendar should be displayed on update.

#### UpdateCalendarInfo

This callback:

```csharp
[ Parameter ]
public EventCallback<CalendarArgs> UpdateCalendarInfo { get; set; }
```

is raised when:

- the calendar is first initialized
- the calendar's `Regenerate()` method was called
- the calendar was moved to a different month

The callback gives the host page a chance
to define what the calendar should look like given its current state. It
does this by passing an instance of [CalendarArgs](calendar-args.md),
which lets the host page define things like background colors for
selected or unselected days.

#### Day of Week Header Cell Was Clicked

This callback:

```csharp
[ Parameter ]
public EventCallback<DowClickedArgs> DayOfWeekClicked { get; set; }
```

is raised when a day of the week header cell is clicked (assuming that
behavior was enabled by setting `DaysOfWeekClickable` to `true`).

The callback gives the host page a chance to update whatever records it
is keeping about the clicked days, and to define how they should appear.
It does this by passing an instance of
[DowClickedArgs](dow-clicked-args.md).

#### Day Clicked

This callback:

```csharp
[ Parameter ]
public EventCallback<CalendarDay> DayClicked { get; set; }
```

is raised when a day within the displayed calendar is clicked. *Whether
or not a given day is clickable depends on how the `div` defining that
day is configured*. I typically do that by assigning a css class to the
`div` making it clickable.

The callback gives the host page a chance to update whatever records it
is keeping about the clicked day, and to define how the clicked day
should appear. It does this by passing an instance of
[CalendarDay](calendar-day.md).

## Usage

You embed the component in a page like this:

```html
<MyCalendar UpdateCalendarInfo="OnUpdateCalendarInfo"
            DayClicked="ToggleDay"
            @key="@_calIdToken"/>

@code {
    private readonly Guid _calIdToken = Guid.NewGuid();

    public void OnUpdateCalendarInfo()
    {
        ...
    }
    ...other code
```

Note also how `@key` is set. This is needed to identify the unique
instance of the component in the running Blazor server app. FWIW,
assigning the GUID directly within the `@key` parameter like this:

```html
<MyCalendar UpdateCalendarInfo="OnUpdateCalendarInfo"
            DayClicked="ToggleDay"
            @key="@(Guid.NewGuid())"/>
```

doesn't work.
