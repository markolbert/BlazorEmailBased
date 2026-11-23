using System.Text.RegularExpressions;

namespace J4JSoftware.BlazorEmailBased;

public partial class OklchColor
{
    public static OklchColor TextToOklchColor( string rgbColor )
    {
        int redRaw;
        int greenRaw;
        int blueRaw;

        try
        {
            redRaw = Convert.ToInt32( rgbColor.Substring( 1, 2 ), 16 );
            greenRaw = Convert.ToInt32( rgbColor.Substring( 3, 2 ), 16 );
            blueRaw = Convert.ToInt32( rgbColor.Substring( 5, 2 ), 16 );
        }
        catch( Exception ex )
        {
            throw new RgbToOklchColorException( rgbColor, ex.Message );
        }

        var red = Inv( redRaw );
        var green = Inv( greenRaw );
        var blue = Inv( blueRaw );

        var l = 0.4122214708 * red + 0.5363325363 * green + 0.0514459929 * blue;
        var m = 0.2119034982 * red + 0.6806995451 * green + 0.1073969566 * blue;
        var s = 0.0883024619 * red + 0.2817188376 * green + 0.6299787005 * blue;

        var lMod = Math.Cbrt( l );
        var mMod = Math.Cbrt( m );
        var sMod = Math.Cbrt( s );

        // 3. LMS to OkLab
        var lightness = 0.2104542553 * lMod + 0.7936177850 * mMod - 0.0040720468 * sMod;
        var a = 1.9779984951 * lMod - 2.4285922050 * mMod + 0.4505937099 * sMod;
        var bb = 0.0259040371 * lMod + 0.7827717662 * mMod - 0.8086757660 * sMod;

        // 4. OkLab to Oklch
        var hue = Math.Atan2( bb, a ) * 180 / Math.PI;
        if( hue < 0 ) hue += 360;

        return new OklchColor { Chroma = Math.Sqrt( a * a + bb * bb ), Hue = hue, Lightness = lightness };

        static double Inv( double c )
        {
            c /= 255.0;
            return c <= 0.04045 ? c / 12.92 : Math.Pow( ( c + 0.055 ) / 1.055, 2.4 );
        }
    }

    public static async Task<Dictionary<int, string>> GetMemberHexRgbColorsAsync(
        List<int> memberIds,
        string cssGlobalsPath,
        string cssColorId,
        double? chromaOverride = null
    )
    {
        var css = await File.ReadAllTextAsync( cssGlobalsPath );

        var match = Regex.Match( css, @$"{cssColorId}:\s*([^;]+);" );
        var backgroundColor = match.Success ? match.Groups[ 1 ].Value.Trim() : "#C6C5DC";

        var lchBackground = OklchColor.TextToOklchColor( backgroundColor );
        var firstComplement = lchBackground.ToSafeComplement();

        var colors = firstComplement.GetContrastingHexRgbColors( memberIds.Count, chromaOverride );

        var retVal = new Dictionary<int, string>();

        var idx = 0;

        foreach( var memberId in memberIds )
        {
            retVal.Add( memberId, colors[ idx ] );

            idx++;
        }

        return retVal;
    }
}
