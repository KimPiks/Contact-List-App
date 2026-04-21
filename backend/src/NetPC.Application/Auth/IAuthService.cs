using NetPC.Application.DTOs.Auth;

namespace NetPC.Application.Auth;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterDto dto);
    Task<AuthResult> LoginAsync(LoginDto dto);
    Task<AuthResult> RefreshAsync(string refreshToken);
    Task<bool> LogoutAsync(Guid userId, string refreshToken);
}