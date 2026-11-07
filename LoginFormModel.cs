using System.ComponentModel.DataAnnotations;

namespace BlazorEmailSiteCommon;

public class LoginFormModel
{
    [ Required( ErrorMessage = "Email field must be filled out" ) ]
    [ EmailAddress( ErrorMessage = "Invalid email address" ) ]
    [ RegularExpression( @"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address" ) ]
    public string? Email { get; set; }
}
