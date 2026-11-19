using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Globalization;

namespace J4JSoftware.BlazorEmailBased;

public sealed class IsoDateOnlyConverter() : ValueConverter<DateOnly?, string?>(
    date => date == null ? null : date.Value.ToString( "yyyy-MM-dd", CultureInfo.InvariantCulture ),
    text => string.IsNullOrEmpty(text) ? null : DateOnly.ParseExact( text, "yyyy-MM-dd", CultureInfo.InvariantCulture ) );