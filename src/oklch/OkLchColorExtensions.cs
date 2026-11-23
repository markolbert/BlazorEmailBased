#pragma warning disable IDE0130
namespace J4JSoftware.BlazorEmailBased;
#pragma warning restore IDE0130

public static class OkLchColorExtensions
{
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

        public List<OklchColor> GetContrastingColors( int numColors, double? chromaOverride = null )
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

        public List<string> GetContrastingHexRgbColors( int numColors, double? chromaOverride = null ) =>
        [
            .. color.GetContrastingColors( numColors, chromaOverride )
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
