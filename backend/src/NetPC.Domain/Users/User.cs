using Microsoft.AspNetCore.Identity;
using NetPC.Domain.RefreshTokens;

namespace NetPC.Domain.Users;

/// <summary>
/// User entity inheriting from IdentityUser to use ASP.NET Identity features
/// </summary>
public class User : IdentityUser<Guid>
{
    public List<RefreshToken> RefreshTokens { get; set; }
}