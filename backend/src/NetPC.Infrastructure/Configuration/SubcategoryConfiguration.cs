using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPC.Domain.Contact;

namespace NetPC.Infrastructure.Configuration;

public class SubcategoryConfiguration : IEntityTypeConfiguration<Subcategory>
{
    public void Configure(EntityTypeBuilder<Subcategory> builder)
    {
        // Define primary key and properties
        builder.HasKey(sc => sc.Id);
        builder.Property(sc => sc.Name).IsRequired().HasMaxLength(128);
        
        // Seed data
        builder.HasData(
            new Subcategory { Id = 1, Name = "Szef", CategoryId = 1 },
            new Subcategory { Id = 2, Name = "Klient", CategoryId = 1 }
        );
    }
}