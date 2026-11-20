#region copyright
// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// LoginToken.cs
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

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

namespace J4JSoftware.BlazorEmailBased.Services;

public class LoginToken
{
    [ Required( ErrorMessage = "Token cannot be blank.", AllowEmptyStrings = false ) ]
    public required string Token { get; set; }

    [ Required( ErrorMessage = "OriginalEmail cannot be blank.", AllowEmptyStrings = false ) ]
    public required string OriginalEmail { get; set; }

    public required DateOnly CreatedUtc { get; set; }
    public required DateOnly ExpiresUtc { get; set; }
    public bool Used { get; set; }
}

public class LoginTokensConfigurator : IEntityTypeConfiguration<LoginToken>
{
    public void Configure( EntityTypeBuilder<LoginToken> builder )
    {
        builder.ToTable( "LoginTokens" );
        builder.HasKey( x => x.Token );

        builder.Property( x => x.Token )
               .IsRequired();

        builder.Property( x => x.OriginalEmail )
               .IsRequired();

        builder.Property( x => x.CreatedUtc )
               .IsRequired();

        builder.Property( x => x.ExpiresUtc )
               .IsRequired();
    }
}
