using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPC.Domain.Contact;

namespace NetPC.Infrastructure.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Define primary key and properties
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        
        // Seed data
        builder.HasData(
            new Category { Id = 1, Name = "Business" },
            new Category { Id = 2, Name = "Private" },
            new Category { Id = 3, Name = "Custom" }
        );
    }
}