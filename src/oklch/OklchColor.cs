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
