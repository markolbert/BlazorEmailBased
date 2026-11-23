namespace J4JSoftware.BlazorEmailBased;

public class RgbToOklchColorException( string text, string mesg )
    : Exception( $"Could not convert '{text}' to {nameof( OklchColor )}, message was '{mesg}'" );
