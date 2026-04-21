using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPC.Domain.Contact;

namespace NetPC.Infrastructure.Configuration;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        // Define primary key and properties
        builder.HasKey(contact => contact.Id);
        builder.Property(contact => contact.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(contact => contact.LastName).IsRequired().HasMaxLength(100);
        builder.Property(contact => contact.Email).HasMaxLength(255);
        builder.Property(contact => contact.PhoneNumber).HasMaxLength(20);
        builder.Property(contact => contact.EncryptedPassword).HasMaxLength(255);
        builder.Property(contact => contact.DateOfBirth).IsRequired();
        
        // Define relationships between tables
        builder
            .HasOne(contact => contact.Category)
            .WithMany()
            .HasForeignKey(contact => contact.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(contact => contact.Subcategory)
            .WithMany()
            .HasForeignKey(contact => contact.SubcategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}