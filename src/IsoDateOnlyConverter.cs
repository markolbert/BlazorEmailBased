using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Globalization;

namespace J4JSoftware.BlazorEmailBased;

public sealed class IsoDateOnlyConverter() : ValueConverter<DateOnly, string>(
    date => date.ToString( "yyyy-MM-dd", CultureInfo.InvariantCulture ),
    text => DateOnly.ParseExact( text, "yyyy-MM-dd", CultureInfo.InvariantCulture ) );