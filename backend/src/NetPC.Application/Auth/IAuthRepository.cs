using NetPC.Domain.RefreshTokens;

namespace NetPC.Application.Auth;

public interface IAuthRepository
{
    Task AddRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken?> GetRefreshTokenAsync(Guid userId, string token);
    Task<RefreshToken?> GetRefreshTokenByValueAsync(string token);
    Task RemoveRefreshTokenAsync(RefreshToken token);
}
