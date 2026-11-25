# CSS Classes Required by MyCalendar

A number of CSS classes need to be defined so `MyCalendar` can
function properly. These are defined in an *isolated CSS file* associated
with `MyCalendar` (**MyCalendar.razor.css** in the source code):

```css
.calendar-card {
    max-width: 500px;
    margin: auto;
    background: #fff;
    border: 1px solid #ddd;
    border-radius: 8px;
    padding: 16px;
}

.calendar-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 12px;
}

.calendar-grid {
    display: grid;
    grid-template-columns: repeat(7, 1fr);
    text-align: center;
}

.weekdays div {
    font-weight: bold;
    color: #666;
    padding-bottom: 8px;
}

.days .calendar-day {
    padding: 12px 0;
    border: 1px solid #f0f0f0;
    cursor: pointer;
}

.days .calendar-day-unclickable {
    padding: 12px 0;
    border: 1px solid #f0f0f0;
}

.days .calendar-day.inactive {
    color: #ccc;
}

.days .calendar-day-unclickable.inactive {
    color: #ccc;
}
```
