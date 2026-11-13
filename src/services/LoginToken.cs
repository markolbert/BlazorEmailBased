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
