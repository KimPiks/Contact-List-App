using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetPC.Domain.Contact;
using NetPC.Domain.RefreshTokens;
using NetPC.Domain.Users;
using NetPC.Infrastructure.Extensions;

namespace NetPC.Infrastructure;

/// <summary>
/// Database context for application
/// </summary>
public class AppDbContext : IdentityUserContext<User, Guid>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Category> ContactCategories { get; set; }
    public DbSet<Subcategory> ContactSubcategories { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Ignore unused Identity tables
        modelBuilder.IgnoreUnusedIdentityTables();
        // Apply configuration for entities (Configuration directory)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}