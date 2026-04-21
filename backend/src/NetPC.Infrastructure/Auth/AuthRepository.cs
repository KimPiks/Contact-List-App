using Microsoft.EntityFrameworkCore;
using NetPC.Application.Auth;
using NetPC.Domain.RefreshTokens;

namespace NetPC.Infrastructure.Auth;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _dbContext;

    public AuthRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRefreshTokenAsync(RefreshToken token)
    {
        _dbContext.RefreshTokens.Add(token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(Guid userId, string token)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == token);
    }

    public async Task<RefreshToken?> GetRefreshTokenByValueAsync(string token)
    {
        return await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task RemoveRefreshTokenAsync(RefreshToken token)
    {
        _dbContext.RefreshTokens.Remove(token);
        await _dbContext.SaveChangesAsync();
    }
}
