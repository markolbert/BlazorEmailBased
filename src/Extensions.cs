namespace J4JSoftware.BlazorEmailBased;

public static class Extensions
{
    public static string DigitsOnly(string? value) =>
        new((value ?? "").Where(char.IsAsciiDigit).ToArray());

    public static string FormatPhone(string? value)
    {
        var digits = DigitsOnly(value);

        return digits.Length == 10
            ? $"({digits[..3]}) {digits[3..6]}-{digits[6..]}"
            : digits;
    }
}
