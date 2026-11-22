#region copyright

// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// CalendarDay.cs
//
// This file is part of JumpForJoy Software's BlazorEmailBased package.
// 
// BlazorEmailBased is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// BlazorEmailBased is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with BlazorEmailBased. If not, see <https://www.gnu.org/licenses/>.

#endregion

namespace J4JSoftware.BlazorEmailBased.Components;

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
