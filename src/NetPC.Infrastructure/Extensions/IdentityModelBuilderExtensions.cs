using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NetPC.Infrastructure.Extensions;

/// <summary>
/// Extension for removing tables from ASP.NET Identity which are not used
/// </summary>
public static class IdentityModelBuilderExtensions
{
    public static void IgnoreUnusedIdentityTables(this ModelBuilder builder)
    {
        builder.Ignore<IdentityUserClaim<string>>();
        builder.Ignore<IdentityUserLogin<string>>();
        builder.Ignore<IdentityUserToken<string>>();
        builder.Ignore<IdentityRole>();
        builder.Ignore<IdentityUserRole<string>>();
        builder.Ignore<IdentityRoleClaim<string>>();
    }
}