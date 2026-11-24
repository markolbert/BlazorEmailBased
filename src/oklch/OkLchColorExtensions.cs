#region copyright
// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// OkLchColorExtensions.cs
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

#pragma warning disable IDE0130
namespace J4JSoftware.BlazorEmailBased;
#pragma warning restore IDE0130

public static class OkLchColorExtensions
{
    public static OklchColor TextToOklchColor(string rgbColor)
    {
        int redRaw;
        int greenRaw;
        int blueRaw;

        try
        {
            redRaw = Convert.ToInt32(rgbColor.Substring(1, 2), 16);
            greenRaw = Convert.ToInt32(rgbColor.Substring(3, 2), 16);
            blueRaw = Convert.ToInt32(rgbColor.Substring(5, 2), 16);
        }
        catch (Exception ex)
        {
            throw new RgbToOklchColorException(rgbColor, ex.Message);
        }

        var red = Inv(redRaw);
        var green = Inv(greenRaw);
        var blue = Inv(blueRaw);

        var l = 0.4122214708 * red + 0.5363325363 * green + 0.0514459929 * blue;
        var m = 0.2119034982 * red + 0.6806995451 * green + 0.1073969566 * blue;
        var s = 0.0883024619 * red + 0.2817188376 * green + 0.6299787005 * blue;

        var lMod = Math.Cbrt(l);
        var mMod = Math.Cbrt(m);
        var sMod = Math.Cbrt(s);

        // 3. LMS to OkLab
        var lightness = 0.2104542553 * lMod + 0.7936177850 * mMod - 0.0040720468 * sMod;
        var a = 1.9779984951 * lMod - 2.4285922050 * mMod + 0.4505937099 * sMod;
        var bb = 0.0259040371 * lMod + 0.7827717662 * mMod - 0.8086757660 * sMod;

        // 4. OkLab to Oklch
        var hue = Math.Atan2(bb, a) * 180 / Math.PI;
        if (hue < 0) hue += 360;

        return new OklchColor { Chroma = Math.Sqrt(a * a + bb * bb), Hue = hue, Lightness = lightness };

        static double Inv(double c)
        {
            c /= 255.0;
            return c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
        }
    }

    extension( OklchColor color )
    {
        public OklchColor ToSafeComplement()
        {
            var hue = color.Hue + 180;
            if( hue >= 360 )
                hue -= 360;

            return new OklchColor
            {
                Chroma = color.Chroma < 0.15 ? 0.15 : color.Chroma,
                Hue = hue,
                Lightness = color.Lightness > 0.6 ? 0.6 : color.Lightness
            };
        }

        public List<OklchColor> GetComplements( int numColors, double? chromaOverride = null )
        {
            if( numColors < 1 )
                numColors = 1;

            var retVal = new List<OklchColor>();

            var cappedChroma = color.Chroma < 0.15 ? 0.15 : color.Chroma;
            if( chromaOverride.HasValue )
                cappedChroma = chromaOverride.Value;

            var step = 360 / numColors;
            var curHue = color.Hue;

            for( var idx = 0; idx < numColors; idx++ )
            {
                retVal.Add( new OklchColor { Chroma = cappedChroma, Hue = curHue, Lightness = color.Lightness } );

                curHue += step;
                if( curHue >= 360 )
                    curHue -= 360;
            }

            return retVal;
        }

        public List<string> GetHexRgbComplements( int numColors, double? chromaOverride = null ) =>
        [
            .. color.GetComplements( numColors, chromaOverride )
                    .Select( lch => lch.ToHexRgb() )
        ];

        public string ToHexRgb( bool includeHash = false )
        {
            var hRad = color.Hue * Math.PI / 180.0;
            var a = color.Chroma * Math.Cos( hRad );
            var b = color.Chroma * Math.Sin( hRad );

            // 2. OkLab to LMS prime
            var lMod = color.Lightness + 0.3963377774 * a + 0.2158037573 * b;
            var mMod = color.Lightness - 0.1055613458 * a - 0.0638541728 * b;
            var sMod = color.Lightness - 0.0894841775 * a - 1.2914855480 * b;

            // 3. LMS prime to LMS to linear RGB
            var l = lMod * lMod * lMod;
            var m = mMod * mMod * mMod;
            var s = sMod * sMod * sMod;

            var r = +4.0767416621 * l - 3.3077115906 * m + 0.2309699292 * s;
            var g = -1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s;
            var bl = -0.0041960863 * l - 0.7034186147 * m + 1.7076147010 * s;

            return $"{( includeHash ? "#" : string.Empty )}{To8Bit( r ):X2}{To8Bit( g ):X2}{To8Bit( bl ):X2}";

            static int To8Bit( double c )
            {
                var sRgb = c <= 0.0031308
                    ? 12.92 * c
                    : 1.055 * Math.Pow( c, 1.0 / 2.4 ) - 0.055;
                return (int) Math.Round( Math.Clamp( sRgb, 0, 1 ) * 255 );
            }
        }
    }
}
