using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPC.Domain.Users;

namespace NetPC.Infrastructure.Configuration;

/// <summary>
/// Configuration for User entity to remove unused properties from ASP.NET Identity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Ignore(u => u.PhoneNumber);
        builder.Ignore(u => u.PhoneNumberConfirmed);
        builder.Ignore(u => u.TwoFactorEnabled);
        builder.Ignore(u => u.LockoutEnabled);
        builder.Ignore(u => u.LockoutEnd);
        builder.Ignore(u => u.AccessFailedCount);
    }
}