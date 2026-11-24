#region copyright
// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// OklchColor.cs
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

namespace J4JSoftware.BlazorEmailBased;

public partial class OklchColor
{
    public const double MinLightness = 0;
    public const double MaxLightness = 1;
    public const double MinChroma = 0;
    public const double MaxChroma = 0.4;
    public const double MinHue = 0;
    public const double MaxHue = 360;

    public double Lightness
    {
        get;

        set
        {
            if( value < MinLightness )
                field = MinLightness;
            else field = field > MaxLightness ? MaxLightness : value;
        }
    }

    public double Chroma
    {
        get;

        set
        {
            if (value < MinChroma)
                field = MinChroma;
            else field = field > MaxChroma ? MaxChroma : value;
        }
    }

    public double Hue
    {
        get;

        set
        {
            if (value < MinHue)
                field = MinHue;
            else field = field > MaxHue ? MaxHue : value;
        }
    }
}
