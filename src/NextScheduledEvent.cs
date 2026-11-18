using System;
using System.Collections.Generic;
using System.Text;

namespace J4JSoftware.BlazorEmailBased;

public sealed class NextScheduledEvent
{
    public event Action? Changed;

    public DateOnly? Date
    {
        get;

        set
        {
            if( field == value )
                return;

            field = value;
            Changed?.Invoke();
        }
    }
}
