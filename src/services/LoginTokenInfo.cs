namespace J4JSoftware.BlazorEmailBased.Services;

public record LoginTokenInfo( LoginToken Token, string VerifyUrl, string LockoutUrl );
