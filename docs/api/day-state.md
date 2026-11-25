# DayState

A flag enum (i.e., more than one value can be set) which controls
whether or not a day cell can be clicked, and whether it is inactive
for some reason (e.g., because it's not a day within the currently
displayed month). Allowed values are:

|Value|Description|
|-----|-----------|
|Clickable|day cell can be clicked|
|Inactive|day cell is inactive|
|None|no defined behavior (default)|

`Inactive` cells can still be made clickable, but that's not a situation
I've ever needed.
