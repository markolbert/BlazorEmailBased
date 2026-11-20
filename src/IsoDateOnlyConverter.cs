#region copyright
// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// IsoDateOnlyConverter.cs
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

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Globalization;

namespace J4JSoftware.BlazorEmailBased;

public sealed class IsoDateOnlyConverter() : ValueConverter<DateOnly?, string?>(
    date => date == null ? null : date.Value.ToString( "yyyy-MM-dd", CultureInfo.InvariantCulture ),
    text => string.IsNullOrEmpty(text) ? null : DateOnly.ParseExact( text, "yyyy-MM-dd", CultureInfo.InvariantCulture ) );