namespace NetPC.Application.DTOs.Auth;

/// <summary>
/// Represents the result of an authentication attempt
/// </summary>
public class AuthResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }

    public string? AccessToken { get; init; }
    public string?  RefreshToken { get; init; }

    public static AuthResult Ok(string accessToken, string refreshToken)
        => new()
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

    public static AuthResult Fail(string error)
        => new()
        {
            Success = false,
            Error = error
        };
}