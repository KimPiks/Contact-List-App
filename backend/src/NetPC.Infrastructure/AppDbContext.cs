using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
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
        modelBuilder.IgnoreUnusedIdentityTables();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Category>()
            .HasIndex(category => category.Name)
            .IsUnique();

        modelBuilder.Entity<Subcategory>()
            .HasIndex(subcategory => new { subcategory.CategoryId, subcategory.Name })
            .IsUnique();

        modelBuilder.Entity<Contact>()
            .HasOne(contact => contact.Category)
            .WithMany()
            .HasForeignKey(contact => contact.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contact>()
            .HasOne(contact => contact.Subcategory)
            .WithMany()
            .HasForeignKey(contact => contact.SubcategoryId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Set default data for contact category
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Business" },
            new Category { Id = 2, Name = "Private" },
            new Category { Id = 3, Name = "Custom" }
        );

        modelBuilder.Entity<Subcategory>().HasData(
            new Subcategory { Id = 1, Name = "Szef", CategoryId = 1 },
            new Subcategory { Id = 2, Name = "Klient", CategoryId = 1 }
        );
    }
}