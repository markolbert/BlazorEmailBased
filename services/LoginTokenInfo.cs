namespace BlazorEmailSiteCommon.Services;

public record LoginTokenInfo( LoginToken Token, string VerifyUrl, string LockoutUrl );
