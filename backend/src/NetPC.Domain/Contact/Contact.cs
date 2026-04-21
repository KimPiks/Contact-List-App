namespace NetPC.Domain.Contact;

public class Contact
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int? SubcategoryId { get; set; }
    public Subcategory? Subcategory { get; set; }
}