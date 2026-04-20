using NetPC.Domain.Users;

namespace NetPC.Domain.RefreshTokens;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public bool IsActive => DateTime.UtcNow < ExpiresAt;
    
    public void AssignUser(Guid userId)
    {
        UserId = userId;
    }
}