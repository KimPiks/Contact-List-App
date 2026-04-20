using NetPC.Domain.RefreshTokens;
using NetPC.Domain.Users;

namespace NetPC.Application.Auth;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken();
}